using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Inventory.API.Extensions
{
    public static class KestrelExtension
    {
        public static WebApplicationBuilder AddCustomKestrel(this WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureEndpointDefaults(listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
            });

            return builder;
        }
    }
}
