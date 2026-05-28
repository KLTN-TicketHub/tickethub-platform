using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Catalog.API.Extensions
{
    public static class SecurityExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var issuer = configuration["Identity:Authority"];
            var audience = configuration["Identity:Audience"];
            var publicKeyPath = configuration["Identity:PublicKeyPath"];

            string fullPath = Path.Combine(
                AppContext.BaseDirectory,
                publicKeyPath!);

            string publicPem = File.ReadAllText(fullPath);

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicPem);

            var rsaSecurityKey = new RsaSecurityKey(rsa);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                //options.Authority = configuration["Identity:Authority"];
                //options.Audience = configuration["Identity:Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Identity:Authority"],

                    ValidateAudience = true,
                    ValidAudience = configuration["Identity:Audience"],

                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaSecurityKey,

                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var response = new
                        {
                            success = false,
                            message = "Bạn cần đăng nhập để truy cập tài nguyên này.",
                            error = new
                            {
                                code = "UNAUTHORIZED_ACCESS",
                                type = "UnauthorizedAccessException"
                            },
                            traceId = context.HttpContext.TraceIdentifier,
                            timestamp = DateTime.UtcNow
                        };

                        return context.Response.WriteAsJsonAsync(response);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = new
                        {
                            success = false,
                            message = "Bạn không có quyền truy cập vào tài nguyên này.",
                            error = new
                            {
                                code = "FORBIDDEN_ACCESS",
                                type = "ForbiddenAccessException"
                            },
                            traceId = context.HttpContext.TraceIdentifier,
                            timestamp = DateTime.UtcNow
                        };

                        return context.Response.WriteAsJsonAsync(response);
                    }
                };
            });

            return services;
        }
    }
}
