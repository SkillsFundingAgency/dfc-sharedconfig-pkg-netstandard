using Autofac;
using Autofac.Extras.DynamicProxy;
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
        public static void AddTableConfigServices(this ContainerBuilder builder, string loggingInterceptorName = null, string exceptionInterceptorName = null)
        {
            if (!string.IsNullOrWhiteSpace(loggingInterceptorName) && !string.IsNullOrWhiteSpace(exceptionInterceptorName))
            {
                builder.RegisterAssemblyTypes(typeof(DIExtensions).Assembly)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();

                builder.RegisterType<AzureTableStorageConfigurationRepository>().As<IConfigurationRepository>()
                    .InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterAssemblyTypes(typeof(DIExtensions).Assembly)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope()
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(loggingInterceptorName, exceptionInterceptorName);

                builder.RegisterType<AzureTableStorageConfigurationRepository>().As<IConfigurationRepository>()
                    .InstancePerLifetimeScope()
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(loggingInterceptorName, exceptionInterceptorName);
            }
        }

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