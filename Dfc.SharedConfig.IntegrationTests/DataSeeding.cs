using Dfc.SharedConfig.Services;
using System.Threading.Tasks;

namespace Dfc.SharedConfig.IntegrationTests
{
    public static class DataSeeding
    {
        public static async Task SeedInitialData(ISharedConfigurationService service)
        {
            if (service != null)
            {
                await service.SetConfigAsync<string>(Constants.IntegrationTestServiceName, Constants.SingleStringKeyName, Constants.SingleStringValue).ConfigureAwait(false);
                await service.SetConfigAsync<SampleConfiguration>(Constants.IntegrationTestServiceName, Constants.SimpleObjectKeyName, Constants.SimpleObjectValue).ConfigureAwait(false);
            }
        }
    }
}