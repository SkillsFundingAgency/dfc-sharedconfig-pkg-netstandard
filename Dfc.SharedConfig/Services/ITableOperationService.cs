using Microsoft.Azure.Cosmos.Table;

namespace Dfc.SharedConfig.Services
{
    internal interface ITableOperationService
    {
        TableOperation GetSharedConfig(string serviceName, string keyName);

        TableOperation SetSharedConfig(string serviceName, string keyName, string data);
    }
}