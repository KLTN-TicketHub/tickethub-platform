using Catalog.API.Protos;
using Inventory.API.Protos;

namespace Ordering.API.Extensions
{
    public static class GrpcExtension
    {
        public static IServiceCollection AddCustomGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddGrpcClient<InventoryGrpc.InventoryGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["GrpcSettings:InventoryUrl"]!);
            });

            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["GrpcSettings:CatalogUrl"]!);
            });

            return services;
        }
    }
}
