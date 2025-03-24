using Qayimli.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Repository.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options ) :base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        //Tables In Our DB
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewDetail> ReviewDetails { get; set; }
        public DbSet<Vote> Votes { get; set; }

        public DbSet<ReviewCategory> ReviewCategories { get; set; } // ReviewCategory 
        public DbSet<ContactUsEmail> ContactUsEmails { get; set; }
    }
    

}
