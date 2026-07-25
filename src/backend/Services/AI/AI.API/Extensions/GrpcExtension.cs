using Catalog.API.Protos;

namespace AI.API.Extensions
{
    public static class GrpcExtension
    {
        public static IServiceCollection AddCustomGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["GrpcSettings:CatalogUrl"]!);
            });

            return services;
        }
    }
}
