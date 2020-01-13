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
    public class SharedConfigurationServiceFileStoreTests
    {
        private readonly ISharedConfigurationService service;

        public SharedConfigurationServiceFileStoreTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var settings = configuration.GetSection("SharedConfigSettings").Get<SharedConfigSettings>();

            var services = new ServiceCollection().AddFileStorageSharedConfigService(settings).BuildServiceProvider();
            service = services.GetService<ISharedConfigurationService>();

            DataSeeding.SeedInitialData(service).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task WhenDataIsNotEncryptedAndIsAStringThenReadStringFromFile()
        {
            // Act
            var result = await service.GetConfigAsync<string>(Constants.IntegrationTestServiceName, Constants.SingleStringKeyName).ConfigureAwait(false);

            // Assert
            Assert.Equal(JsonConvert.DeserializeObject<string>(Constants.SingleStringValue), result);
        }

        [Fact]
        public async Task WhenDataIsNotEncryptedAndIsAnObjectThenReadObjectFromFile()
        {
            // Act
            var result = await service.GetConfigAsync<SampleConfiguration>(Constants.IntegrationTestServiceName, Constants.SimpleObjectKeyName).ConfigureAwait(false);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(result.DataField1) &&
                !string.IsNullOrWhiteSpace(result.DataField2) &&
                !string.IsNullOrWhiteSpace(result.DataField3));
        }
    }
}