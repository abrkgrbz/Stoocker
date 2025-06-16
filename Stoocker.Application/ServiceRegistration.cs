using Microsoft.Extensions.DependencyInjection;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Stoocker.Application.Behaviors;

namespace Stoocker.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // AutoMapper
            services.AddAutoMapper(assembly);

            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);

            // MediatR Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITenantService, TenantService>();

            services.AddHttpContextAccessor();

        }
    }
}
