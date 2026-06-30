using Catalog.API.Protos;
using Inventory.API.Protos;

namespace Ordering.API.Extensions
{
    public static class GrpcExtension
    {
        public static IServiceCollection AddCustomGrpc(this IServiceCollection services, IConfiguration configuration)
        {
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
