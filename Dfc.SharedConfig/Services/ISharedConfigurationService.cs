using System.Threading.Tasks;

namespace Dfc.SharedConfig.Services
{
    public interface ISharedConfigurationService
    {
        Task<T> GetConfigAsync<T>(string serviceName, string keyName, bool isDataEncrypted = false);

        Task SetConfigAsync<T>(string serviceName, string keyName, string data, bool isDataEncrypted = false);
    }
}