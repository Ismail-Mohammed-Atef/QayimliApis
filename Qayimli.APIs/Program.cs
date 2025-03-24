using Qayimli.APIs.Errors;
using Qayimli.APIs.Extensions;
using Qayimli.APIs.Helpers;
using Qayimli.APIs.Middlewares;
using Qayimli.Core.Entities;
using Qayimli.Core.Entities.Identity;
using Qayimli.Core.RepositoriesContract;
using Qayimli.Repository;
using Qayimli.Repository.Data;
using Qayimli.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Qayimli.APIs
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSwaggerServices();

            #region Configuration Service
            builder.Services.AddControllers();

            builder.Services.AddApplicationServices();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(Option =>
            {
                var connection = (builder.Configuration.GetConnectionString("RedisConnection"));
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.WithOrigins(builder.Configuration["AllowFrontURLS"].Split(","))
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });
            #endregion

            var app = builder.Build();

            #region UpdateDatabase
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var _dbContext = services.GetRequiredService<StoreContext>();
                await _dbContext.Database.MigrateAsync();

                var identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
                await identityDbContext.Database.MigrateAsync();

                await StoreContextSeed.SeedAsync(_dbContext);

                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(userManager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error has occurred during the migration");
            }
            #endregion

            #region Configure Kestrel Middlewares
            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    //  app.UseDeveloperExceptionPage();    
            //    app.UseSwaggerMiddlewares();
            //}

            //  app.UseDeveloperExceptionPage();    
            app.UseSwaggerMiddlewares();

            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}