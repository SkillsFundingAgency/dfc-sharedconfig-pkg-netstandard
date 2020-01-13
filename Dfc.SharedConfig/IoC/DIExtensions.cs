using Dfc.SharedConfig.Cache;
using Dfc.SharedConfig.Contracts;
using Dfc.SharedConfig.Models;
using Dfc.SharedConfig.Repositories;
using Dfc.SharedConfig.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Dfc.SharedConfig.IoC
{
    [ExcludeFromCodeCoverage]
    public static class DIExtensions
    {
        public static IServiceCollection AddAzureTableSharedConfigService(this IServiceCollection services, SharedConfigSettings sharedConfigSettings)
        {
            services.AddSingleton<IConfigurationRepository, AzureTableStorageConfigurationRepository>();
            return AddServices(services, sharedConfigSettings);
        }

        public static IServiceCollection AddFileStorageSharedConfigService(this IServiceCollection services, SharedConfigSettings sharedConfigSettings)
        {
            services.AddSingleton<IConfigurationRepository, FileStorageConfigurationRepository>();
            return AddServices(services, sharedConfigSettings);
        }

        private static IServiceCollection AddServices(IServiceCollection services, SharedConfigSettings sharedConfigSettings)
        {
            services.AddSingleton(sharedConfigSettings);
            services.AddSingleton<ISharedConfigCacheProvider>(s => new InMemorySharedConfigCacheProvider(sharedConfigSettings));
            services.AddSingleton<ITableOperationService, TableOperationService>();
            services.AddSingleton<ISharedConfigurationService, SharedConfigurationService>();

            return services;
        }
    }
}