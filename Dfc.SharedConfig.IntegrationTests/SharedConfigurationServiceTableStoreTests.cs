using Dfc.SharedConfig.IoC;
using Dfc.SharedConfig.Models;
using Dfc.SharedConfig.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.SharedConfig.IntegrationTests
{
    public class SharedConfigurationServiceTableStoreTests
    {
        private readonly ISharedConfigurationService service;

        public SharedConfigurationServiceTableStoreTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var settings = configuration.GetSection("SharedConfigSettings").Get<SharedConfigSettings>();

            var services = new ServiceCollection().AddAzureTableSharedConfigService(settings).BuildServiceProvider();
            service = services.GetService<ISharedConfigurationService>();

            DataSeeding.SeedInitialData(service).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task WhenDataIsNotEncryptedAndIsAStringThenReadStringFromTableStorage()
        {
            // Act
            var result = await service.GetConfigAsync<string>(Constants.IntegrationTestServiceName, Constants.SingleStringKeyName, false).ConfigureAwait(false);

            // Assert
            Assert.Equal(JsonConvert.DeserializeObject<string>(Constants.SingleStringValue), result);
        }

        [Fact]
        public async Task WhenDataIsNotEncryptedAndIsAnObjectThenReadObjectFromTableStorage()
        {
            // Act
            var result = await service.GetConfigAsync<SampleConfiguration>(Constants.IntegrationTestServiceName, Constants.SimpleObjectKeyName, false).ConfigureAwait(false);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(result.DataField1) &&
                !string.IsNullOrWhiteSpace(result.DataField2) &&
                !string.IsNullOrWhiteSpace(result.DataField3));
        }
    }
}