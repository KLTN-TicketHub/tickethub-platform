using BuildingBlocks.Contracts.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace BuildingBlocks.API.Extensions
{
    public static class RateLimitExtension
    {
        public static IServiceCollection AddCustomRateLimit(
            this IServiceCollection services,
            RateLimitConfig? rateLimitConfig)
        {
            int authenticatedLimit = rateLimitConfig?.AuthenticatedUser?.PermitLimit ?? 200;
            int authenticatedWindow = rateLimitConfig?.AuthenticatedUser?.WindowMinutes ?? 1;

            int anonymousLimit = rateLimitConfig?.AnonymousUser?.PermitLimit ?? 50;
            int anonymousWindow = rateLimitConfig?.AnonymousUser?.WindowMinutes ?? 1;

            int loginLimit = rateLimitConfig?.LoginAttempts?.PermitLimit ?? 5;
            int loginWindow = rateLimitConfig?.LoginAttempts?.WindowMinutes ?? 15;

            services.AddRateLimiter(options =>
            {
                options.AddPolicy(RateLimitPolicies.PerIp, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = anonymousLimit,
                            Window = TimeSpan.FromMinutes(anonymousWindow)
                        }
                    )
                );

                options.AddPolicy(RateLimitPolicies.PerUser, httpContext =>
                {
                    string? userId = GetUserIdFromJwt(httpContext);

                    if (!string.IsNullOrEmpty(userId))
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: $"user:{userId}",
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = authenticatedLimit,
                                Window = TimeSpan.FromMinutes(authenticatedWindow)
                            }
                        );
                    }

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = anonymousLimit,
                            Window = TimeSpan.FromMinutes(anonymousWindow)
                        }
                    );
                });

                options.AddPolicy(RateLimitPolicies.Login, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = loginLimit,
                            Window = TimeSpan.FromMinutes(loginWindow)
                        }
                    )
                );

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Quá nhiều yêu cầu đã được gửi. Vui lòng thử lại sau.",
                        error = new
                        {
                            code = "RATE_LIMIT_EXCEEDED",
                            type = "RateLimitExceededException"
                        },
                        traceId = context.HttpContext.TraceIdentifier,
                        timestamp = DateTime.UtcNow
                    }, cancellationToken: token);
                };
            });

            return services;
        }

        private static string? GetUserIdFromJwt(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User?.FindFirst("sub")?.Value
                ?? httpContext.User?.FindFirst("userId")?.Value
                ?? httpContext.User?.FindFirst("nameid")?.Value;

            return userIdClaim;
        }
    }
    public static class RateLimitPolicies
    {
        public const string PerUser = "per-user";

        public const string PerIp = "per-ip";

        public const string Login = "login";
    }
}
