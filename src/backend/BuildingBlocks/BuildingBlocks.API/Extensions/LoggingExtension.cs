using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.API.Extensions
{
    public static class LoggingExtension
    {
        public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration, string serviceName)
        {
            services.AddSerilog((sp, loggerConfig) =>
            {
                loggerConfig
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Service", serviceName)
                    .WriteTo.Console();

                var seqUrl = configuration["Serilog:SeqUrl"];
                if (!string.IsNullOrWhiteSpace(seqUrl))
                {
                    loggerConfig.WriteTo.Seq(seqUrl);
                }
            });

            return services;
        }
    }
}
