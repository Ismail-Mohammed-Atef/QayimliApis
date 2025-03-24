using Qayimli.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qayimli.Core.Entities;

namespace Qayimli.Repository.Identity
{
    public class AppIdentityDbContext:IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext>options):base(options)
        {
            
        }
    
    }
}
