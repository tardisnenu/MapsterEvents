using Microsoft.Extensions.DependencyInjection;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Service.Services;
using MapsterEvents.Service.Mappings;

namespace MapsterEvents.Service
{
    /// <summary>
    /// Servis katmanı dependency injection uzantıları
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Servis katmanı bağımlılıklarını ekler
        /// </summary>
        /// <param name="services">Servis koleksiyonu</param>
        /// <returns>Servis koleksiyonu</returns>
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            // Servis bağımlılıkları
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthService, AuthService>();

            // Mapster yapılandırması
            ConfigureMappings();

            return services;
        }

        /// <summary>
        /// Mapster mapping yapılandırmasını uygular
        /// </summary>
        private static void ConfigureMappings()
        {
            EventMappingConfig.ConfigureEventMappings();
            CategoryMappingConfig.ConfigureCategoryMappings();
            UserMappingConfig.ConfigureUserMappings();
        }
    }
}