using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Qayimli.Core.Entities.Identity;
using Qayimli.Core.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Create JWT token for the user
    public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
    {
        var authUserClaims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var userRoles = await userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authUserClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var userDetaills = new
        {
            displayName = user.DisplayName,
            email = user.Email,
            pictureUrl = user.PictureUrl
        };

        authUserClaims.Add(new Claim("userDetaills", JsonConvert.SerializeObject(userDetaills)));

        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
            claims: authUserClaims,
            signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Create a password reset token with a short expiration (1 hour)
    public async Task<string> CreatePasswordResetToken(AppUser user)
    {
        var resetClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("reset", "true") // Custom claim to identify as a reset token
        };

        var resetKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        var resetToken = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(1),
            claims: resetClaims,
            signingCredentials: new SigningCredentials(resetKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(resetToken);
    }

    // Validate the reset token
    public ClaimsPrincipal ValidateResetToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                ClockSkew = TimeSpan.Zero // Disable clock skew
            }, out SecurityToken validatedToken);

            var resetClaim = principal.FindFirst("reset");
            if (resetClaim == null || resetClaim.Value != "true")
            {
                Console.WriteLine("Token validation failed: 'reset' claim is missing or incorrect");
                return null; // Invalid token
            }

            return principal; // Token is valid with the correct claims
        }
        catch (SecurityTokenExpiredException)
        {
            Console.WriteLine("Token validation failed: Token has expired");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            Console.WriteLine("Token validation failed: Invalid signature");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null;
        }
    }
}
