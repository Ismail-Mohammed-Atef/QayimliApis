using Qayimli.APIs.Errors;
using Qayimli.APIs.Helpers;
using Qayimli.Core;
using Qayimli.Core.RepositoriesContract;
using Qayimli.Core.Service;
using Qayimli.Repository;
using Qayimli.Service;
using Microsoft.AspNetCore.Mvc;

namespace Qayimli.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();//AllowDependencyInjection

            services.AddScoped<IUnitOfWork, UnitOfWork>();//AllowDependencyInjection

            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddAutoMapper(typeof(MappingProfile));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToArray();

                    var validationErrorRespone = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(validationErrorRespone);
                };

            });
            return services;
        }
    }
}
