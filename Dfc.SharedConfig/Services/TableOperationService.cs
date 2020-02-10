using Dfc.SharedConfig.Models;
using Microsoft.Azure.Cosmos.Table;
using System.Diagnostics.CodeAnalysis;

namespace Dfc.SharedConfig.Services
{
    [ExcludeFromCodeCoverage]
    internal class TableOperationService : ITableOperationService
    {
        public TableOperation GetSharedConfig(string serviceName, string keyName)
        {
            return TableOperation.Retrieve<TableStorageItem>(serviceName, keyName);
        }

        public TableOperation SetSharedConfig(string serviceName, string keyName, string data)
        {
            var tableentity = new TableStorageItem
            {
                PartitionKey = serviceName,
                RowKey = keyName,
                Data = data,
            };

            return TableOperation.InsertOrReplace(tableentity);
        }
    }
}