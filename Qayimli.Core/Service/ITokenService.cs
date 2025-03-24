using Qayimli.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Qayimli.Core.Service
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager);
        Task<string> CreatePasswordResetToken(AppUser user);
        ClaimsPrincipal ValidateResetToken(string token);
    }
}
