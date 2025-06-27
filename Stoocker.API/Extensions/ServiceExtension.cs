using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stoocker.Domain.Entities;
using Stoocker.Persistence.Contexts;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Stoocker.API.Handlers;
using Stoocker.API.Providers;
using System.Security.Claims;
using Stoocker.API.Filter;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stoocker.API.Extensions
{
    public static class ServiceExtension
    {
        public static void AddSwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {  // Ana API dokümantasyonu
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Stoocker API",
                    Version = "v1",
                    Description = "Stoocker Multi-Tenant Inventory Management System API",
                    Contact = new OpenApiContact
                    {
                        Name = "Stoocker Team",
                        Email = "support@stoocker.com"
                    }
                });
                // Admin API dokümantasyonu
                c.SwaggerDoc("admin", new OpenApiInfo
                {
                    Title = "Stoocker Admin API",
                    Version = "v1",
                    Description = "Stoocker Super Admin Panel API - Restricted Access",
                    Contact = new OpenApiContact
                    {
                        Name = "Stoocker Admin",
                        Email = "admin@stoocker.com"
                    }
                });


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
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

                c.DocInclusionPredicate((docName, apiDesc) =>
               {
                     if (!apiDesc.TryGetMethodInfo(out var methodInfo))
                         return false;

                   // Controller'ın area attribute'unu kontrol et
                     var controllerAttributes = methodInfo.DeclaringType?.GetCustomAttributes(true);
                     var areaAttribute = controllerAttributes?
                         .OfType<Microsoft.AspNetCore.Mvc.AreaAttribute>()
                         .FirstOrDefault();

                     if (docName == "admin")
                     {
                       // Admin area controller'ları
                         return areaAttribute?.RouteValue == "Admin";
                     }
                     else if (docName == "v1")
                     {
                       // Admin olmayan controller'lar
                         return areaAttribute == null || areaAttribute.RouteValue != "Admin";
                     }

                     return false;
                 });

                c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Cookie,
                    Name = "admin-auth-token",
                    Description = "Admin authentication cookie"
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.OperationFilter<SwaggerAreaOperationFilter>();
                c.EnableAnnotations();
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
                          ClockSkew = TimeSpan.Zero,
                          RoleClaimType = ClaimTypes.Role,
                          NameClaimType = ClaimTypes.Name
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
                              if (context.Request.Path.StartsWithSegments("/api/admin"))
                              {
                                  if (context.Request.Cookies.TryGetValue("admin-auth-token", out var adminToken))
                                  {
                                      context.Token = adminToken;
                                  }
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

        public static void AddPermissionAuthorizationHandlerExtension(this IServiceCollection services)
        {
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
        
    }
}
