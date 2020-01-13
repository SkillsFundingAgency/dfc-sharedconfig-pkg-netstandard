using Dfc.SharedConfig.Models;
using Dfc.SharedConfig.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Dfc.SharedConfig.IntegrationTests")]

namespace Dfc.SharedConfig.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class AzureTableStorageConfigurationRepository : IConfigurationRepository
    {
        private readonly ITableOperationService tableOperationService;
        private readonly SharedConfigSettings settings;

        public AzureTableStorageConfigurationRepository(SharedConfigSettings settings, ITableOperationService tableOperationService)
        {
            this.settings = settings;
            this.tableOperationService = tableOperationService;
        }

        public async Task<string> GetCloudConfigAsync(string serviceName, string keyName)
        {
            var table = await this.GetCloudTable().ConfigureAwait(false);
            var operation = this.tableOperationService.GetSharedConfig(serviceName, keyName);
            var result = await table.ExecuteAsync(operation).ConfigureAwait(false);

            var configItem = (TableStorageItem)result.Result;
            return configItem?.Data;
        }

        public async Task<string> SetCloudConfigAsync(string serviceName, string keyName, string data)
        {
            var table = await this.GetCloudTable().ConfigureAwait(false);
            var operation = this.tableOperationService.SetSharedConfig(serviceName, keyName, data);
            var result = await table.ExecuteAsync(operation).ConfigureAwait(false);

            var configItem = (TableStorageItem)result.Result;
            return configItem?.Data;
        }

        private async Task<CloudTable> GetCloudTable()
        {
            var storageAccount = CloudStorageAccount.Parse(this.settings?.ConfigurationStorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(this.settings?.CloudStorageTableName);
            if (this.settings.EnvironmentName.ToUpperInvariant().Contains("DEV"))
            {
                await table.CreateIfNotExistsAsync().ConfigureAwait(false);
            }

            return table;
        }
    }
}