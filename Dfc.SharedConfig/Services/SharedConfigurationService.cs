using Dfc.SharedConfig.Contracts;
using Dfc.SharedConfig.Repositories;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Dfc.SharedConfig.Services
{
    public class SharedConfigurationService : ISharedConfigurationService
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly ISharedConfigCacheProvider sharedConfigCacheProvider;

        public SharedConfigurationService(IConfigurationRepository configurationRepository, ISharedConfigCacheProvider sharedConfigCacheProvider)
        {
            this.configurationRepository = configurationRepository;
            this.sharedConfigCacheProvider = sharedConfigCacheProvider;
        }

        public async Task<T> GetConfigAsync<T>(string serviceName, string keyName, bool isDataEncrypted = false)
        {
            if (isDataEncrypted)
            {
                throw new NotImplementedException();
            }

            var cacheDetails = this.sharedConfigCacheProvider?.GetConfig($"{serviceName}_{keyName}");
            if (!string.IsNullOrEmpty(cacheDetails))
            {
                return ParseConfig<T>(cacheDetails);
            }

            var repositoryDetails = await this.configurationRepository.GetCloudConfigAsync(serviceName, keyName).ConfigureAwait(false);
            var repositoryResult = ParseConfig<T>(repositoryDetails);
            if (!string.IsNullOrEmpty(repositoryDetails))
            {
                this.sharedConfigCacheProvider?.SetConfig($"{serviceName}_{keyName}", repositoryResult);
            }

            return repositoryResult;
        }

        public async Task SetConfigAsync<T>(string serviceName, string keyName, string data, bool isDataEncrypted = false)
        {
            if (isDataEncrypted)
            {
                throw new NotImplementedException();
            }

            var repositoryDetails = await this.configurationRepository.SetCloudConfigAsync(serviceName, keyName, data).ConfigureAwait(false);
            var repositoryResult = ParseConfig<T>(repositoryDetails);
            if (!string.IsNullOrEmpty(repositoryDetails))
            {
                this.sharedConfigCacheProvider?.SetConfig($"{serviceName}_{keyName}", repositoryResult);
            }
        }

        private static T ParseConfig<T>(string details)
        {
            return string.IsNullOrEmpty(details) ? default(T) : JsonConvert.DeserializeObject<T>(details);
        }
    }
}