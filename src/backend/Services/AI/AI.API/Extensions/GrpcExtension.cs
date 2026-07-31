using Catalog.API.Protos;
using Ordering.API.Protos;

namespace AI.API.Extensions
{
    public static class GrpcExtension
    {
        public static IServiceCollection AddCustomGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["GrpcSettings:CatalogUrl"]!);
            });

            services.AddGrpcClient<OrderingGrpc.OrderingGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["GrpcSettings:OrderingUrl"]!);
            });

            return services;
        }
    }
}
