using AutoMapper;
using Qayimli.APIs.Errors;
using Qayimli.APIs.Extensions;
using Qayimli.Core.Entities.Identity;
using Qayimli.Core.Service;
using Qayimli.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Qayimli.APIs.Dtos.Requests;
using Qayimli.APIs.Dtos.Responses;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.Data;
using System.Net;

namespace Qayimli.APIs.Controllers
{
    public class AccountsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        // Register User
        [HttpPost("Register")]
        public async Task<ActionResult<UserResponseDto>> Register(RegisterRequestDto registerModel)
        {
            if (CheckEmailExist(registerModel.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "this Email Exist"));
            }

            var user = new AppUser()
            {
                DisplayName = registerModel.DisplayName,
                Email = registerModel.Email,
                PictureUrl = registerModel.PictureUrl,
                UserName = registerModel.Email.Split('@')[0],
                PhoneNumber = registerModel.PhoneNumber,
            };

            var regRequestResult = await _userManager.CreateAsync(user, registerModel.Password);
            var mappedUser = _mapper.Map<UserResponseDto>(user); // using automapper
            mappedUser.Token = await _tokenService.CreateTokenAsync(user, _userManager);

            return !regRequestResult.Succeeded ? BadRequest(new ApiResponse(400)) : Ok(mappedUser);
        }

        // Login User
        [HttpPost("Login")]
        public async Task<ActionResult<UserResponseDto>> Login(LoginRequestDto loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user is null) return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);
            var mappedUser = _mapper.Map<UserResponseDto>(user);
            mappedUser.Token = await _tokenService.CreateTokenAsync(user, _userManager);

            return !result.Succeeded ? Unauthorized(new ApiResponse(401)) : Ok(mappedUser);
        }

        [HttpPost("GoogleCallback")]
        public async Task<ActionResult<UserResponseDto>> GoogleCallback([FromBody] GoogleLoginRequest googleAuth)
        {
            if (string.IsNullOrEmpty(googleAuth.Token))
            {
                return BadRequest(new ApiResponse(400, "Token Is Empty"));
            }
            // Validate the ID token with Google
            var (payload, validationError) = await ValidateGoogleTokenAsync(googleAuth.Token);
            if (payload == null)
            {
                return BadRequest(new ApiResponse(400, $"Invalid Google token: {validationError}"));
            }

            // Retrieve user's email from Google
            var email = payload.Email;

            // Check if the user already exists in the system
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                // If not, register a new user based on Google credentials
                user = new AppUser
                {
                    DisplayName = payload.Name,
                    Email = email,
                    UserName = email.Split('@')[0],
                    PictureUrl = payload.Picture // Google profile picture
                };
                await _userManager.CreateAsync(user);
            }

            // Generate JWT Token for the user
            var mappedUser = _mapper.Map<UserResponseDto>(user);
            mappedUser.Token = await _tokenService.CreateTokenAsync(user, _userManager);

            return Ok(mappedUser);
        }
        private async Task<(GoogleJsonWebSignature.Payload payload, string error)> ValidateGoogleTokenAsync(string token)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["Google:ClientId"] } // Replace with your Google client ID
                };

                // Validate the token
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return (payload, null); // Valid token
            }
            catch (InvalidJwtException ex)
            {
                // Detailed error handling based on exception
                return (null, $"Token validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                // General error (could be network issues, etc.)
                return (null, $"Unexpected error during validation: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            var mappedUser = _mapper.Map<UserResponseDto>(user);
            mappedUser.Token = await _tokenService.CreateTokenAsync(user, _userManager);
            return Ok(mappedUser);
        }

        [Authorize]
        [HttpGet("UserAddress")] // Get User Address
        public async Task<ActionResult<UserAddressResponseDto>> GetCurrentUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            var mappedAddress = _mapper.Map<UserAddress, UserAddressResponseDto>(user.UserAddress);
            return Ok(mappedAddress);
        }

        [Authorize]
        [HttpPut("UserAddress")] // Update User Address
        public async Task<ActionResult<UserAddressResponseDto>> UpdateUserAddress(UserAddressRequestDto updatedAddress)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            if (user is null) return Unauthorized(new ApiResponse(401));

            var address = _mapper.Map<UserAddressRequestDto, UserAddress>(updatedAddress);
            user.UserAddress = address;
            var updateAddress = await _userManager.UpdateAsync(user);

            return updateAddress.Succeeded ? Ok(updateAddress) : BadRequest(new ApiResponse(400));
        }

        [HttpGet("EmailExist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        [Authorize]
        [HttpGet("GetUserByEmail/{email}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new ApiResponse(404, "User not found"));

            var mappedUser = _mapper.Map<UserResponseDto>(user);
            mappedUser.Token = await _tokenService.CreateTokenAsync(user, _userManager); // if needed
            return Ok(mappedUser);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Generate reset token
            var resetToken = await _tokenService.CreatePasswordResetToken(user);
            var resetUrl = $"{_configuration["FrontBaseURL"]}/resetpassword/{resetToken}";

            // Send email with reset link
            var subject = "Qayimli - Reset Password";
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "HTMLFiles", "ResetPasswordMail.html");
            var body = await System.IO.File.ReadAllTextAsync(templatePath);
            body = body.Replace("{{resetUrl}}", resetUrl);

            await _emailService.SendEmailAsync(request.Email, subject, body);

            return Ok(new { Message = "Password reset link sent to your email." });
        }

        // API to reset the password
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            // Validate and decode the token
            var principal = _tokenService.ValidateResetToken(request.Token);
            if (principal == null)
            {
                return BadRequest("Invalid or expired reset token.");
            }

            var emailClaim = principal.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                return BadRequest("Invalid reset token.");
            }

            var user = await _userManager.FindByEmailAsync(emailClaim.Value);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Reset password
            // Generate password reset token using ASP.NET Identity
            var identityResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Use the Identity-generated token for password reset
            var result = await _userManager.ResetPasswordAsync(user, identityResetToken, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Failed to reset password: {errors}");
            }

            return Ok(new { Message = "Password reset successful" });
        }
    }
}
