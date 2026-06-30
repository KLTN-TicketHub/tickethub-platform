using Catalog.API.Protos;

namespace Inventory.API.Extensions
{
    public static class GrpcClientExtension
    {
        public static IServiceCollection AddCustomGrpcClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["GrpcSettings:CatalogUrl"]!);
            });

            return services;
        }
    }
}
