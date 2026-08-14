using AI.Infrastructure.Interfaces.IServices;
using AI.Infrastructure.Services;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;

namespace AI.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICatalogAiClient, CatalogAiClient>();
            services.AddScoped<IOrderingAiClient, OrderingAiClient>();
            services.AddScoped<ILlmClient, OpenRouterLlmClient>();
            services.AddScoped<ISuggestionService, SuggestionService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddHttpClient();

            return services;
        }
    }
}
