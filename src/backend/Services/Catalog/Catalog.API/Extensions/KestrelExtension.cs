using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Catalog.API.Extensions
{
    public static class KestrelExtension
    {
        public static WebApplicationBuilder AddCustomKestrel(this WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
            });

            return builder;
        }
    }
}
