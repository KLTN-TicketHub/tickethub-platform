using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Catalog.API.Extensions
{
    public static class SecurityExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["Identity:Authority"];
                    options.Audience = configuration["Identity:Audience"];
                });

            return services;
        }
    }
}
