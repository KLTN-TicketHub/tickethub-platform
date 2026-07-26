using Catalog.API.Protos;

namespace Inventory.API.Extensions
{
    public static class GrpcClientExtension
    {
        public static IServiceCollection AddCustomGrpcClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["GrpcSettings:CatalogUrl"]!);
            });

            return services;
        }
    }
}
