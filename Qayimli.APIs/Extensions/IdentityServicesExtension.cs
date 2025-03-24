using Qayimli.Core.Entities.Identity;
using Qayimli.Core.Service;
using Qayimli.Repository.Identity;
using Qayimli.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

namespace Qayimli.APIs.Extensions
{
    public static class IdentityServicesExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddIdentity<AppUser, IdentityRole>(Options =>
            {
                Options.Password.RequireDigit = false; // Require at least one digit
                Options.Password.RequireLowercase = false; // Require at least one lowercase letter
                Options.Password.RequireUppercase = false; // Require at least one uppercase letter
                Options.Password.RequireNonAlphanumeric = false; // Require at least one non-alphanumeric character
                Options.Password.RequiredLength = 8; // Minimum length of 8
                Options.Password.RequiredUniqueChars = 0; // Number of unique characters

            }).AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
            services.AddAuthentication(Options=>
            {
                Options.DefaultAuthenticateScheme= JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Google:ClientId"];
                googleOptions.ClientSecret = configuration["Google:ClientSecret"];
                googleOptions.CallbackPath = "/Accounts/GoogleCallback"; // Set your callback path here
            })
           .AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))

                };
            });
            services.AddSwaggerGen(swagger =>
            {
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                });
            });


            return services;
        }
    }
}
