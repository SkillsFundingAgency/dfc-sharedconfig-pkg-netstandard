using System.Threading.Tasks;

namespace Dfc.SharedConfig.Repositories
{
    public interface IConfigurationRepository
    {
        Task<string> GetCloudConfigAsync(string serviceName, string keyName);

        Task<string> SetCloudConfigAsync(string serviceName, string keyName, string data);
    }
}