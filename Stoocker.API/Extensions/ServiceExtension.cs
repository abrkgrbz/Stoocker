using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stoocker.Domain.Entities;
using Stoocker.Persistence.Contexts;
using System.Text;

namespace Stoocker.API.Extensions
{
    public static class ServiceExtension
    {
        public static void AddSwaggerExtension(this IServiceCollection services)
        {
           services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Stoocker API", Version = "v1" });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });
            });

        }

        public static void AddIdentityCoreExtension(this IServiceCollection services)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;

                    // Email confirmation
                    options.SignIn.RequireConfirmedEmail = false; // Development için false
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddAuthenticationExtension(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing");
            var issuer = jwtSettings["Issuer"] ?? "Stoocker";
            var audience = jwtSettings["Audience"] ?? "Stoocker";

            if (secretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
            }
           services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                 .AddJwtBearer(options =>
                 {
                     options.SaveToken = true;
                     options.RequireHttpsMetadata = false; // Set to true in production
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                         ValidateIssuer = true,
                         ValidIssuer = issuer,
                         ValidateAudience = true,
                         ValidAudience = audience,
                         ClockSkew = TimeSpan.Zero
                     };
                     options.Events = new JwtBearerEvents
                     {
                         OnMessageReceived = context =>
                         {
                             var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                             var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                             if (!string.IsNullOrEmpty(token))
                             {
                                 logger.LogInformation($"Token received: {token.Substring(0, Math.Min(token.Length, 20))}...");
                             }

                             return Task.CompletedTask;
                         },
                         OnAuthenticationFailed = context =>
                         {
                             var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                             logger.LogError($"Authentication failed: {context.Exception.Message}");
                             return Task.CompletedTask;
                         },
                         OnTokenValidated = context =>
                         {
                             var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                             logger.LogInformation($"Token validated for user: {context.Principal?.Identity?.Name}");
                             return Task.CompletedTask;
                         }
                     };
                 });
        }

    }
}
