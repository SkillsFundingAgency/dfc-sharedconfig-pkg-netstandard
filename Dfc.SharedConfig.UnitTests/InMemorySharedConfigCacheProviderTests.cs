using Dfc.SharedConfig.Cache;
using Dfc.SharedConfig.Models;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Dfc.SharedConfig.UnitTests
{
    public class InMemorySharedConfigCacheProviderTests
    {
        private const string SingleValueKey = "singleValue";
        private const string SimpleObjectKey = "simpleObject";

        private readonly InMemorySharedConfigCacheProvider configCacheProvider;

        public InMemorySharedConfigCacheProviderTests()
        {
            var settings = new SharedConfigSettings
            {
                ConfigurationStorageConnectionString = "DummyConnectionString",
                InMemoryCacheTimeToLiveTimeSpan = "00:01:00",
                CloudStorageTableName = "CloudTableName",
                EnvironmentName = "Develop"
            };

            configCacheProvider = new InMemorySharedConfigCacheProvider(settings);
            SetDefaultCacheValues(configCacheProvider);
        }

        [Fact]
        public void GetConfigReturnsEmptyStringWhenKeyNotFound()
        {
            // Act
            var result = configCacheProvider.GetConfig("UnknownKey");

            // Assert
            Assert.True(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void GetConfigReturnsEmptyStringWhenCacheItemExpired()
        {
            // Act
            var result = configCacheProvider.GetConfig(SingleValueKey);

            // Assert
            Assert.True(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void GetConfigReturnsDataWhenCacheItemNotExpired()
        {
            // Act
            var result = configCacheProvider.GetConfig(SimpleObjectKey);

            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }
        
        [Fact]
        public void SetConfig()
        {
            // Arrange
            var newCacheItemKey = Guid.NewGuid().ToString();

            // Act
            configCacheProvider.SetConfig(newCacheItemKey, "SomeData");

            // Assert
            Assert.True(configCacheProvider.Cache.ContainsKey(newCacheItemKey));
        }




        private void SetDefaultCacheValues(InMemorySharedConfigCacheProvider provider)
        {
            var expiredSingleValue = new InMemoryCacheItem
            {
                Data = JsonConvert.SerializeObject("testValue"),
                ExpiryDate = DateTime.Now.AddSeconds(-60),
            };

            var simpleObjectCacheItem = new InMemoryCacheItem
            {
                Data = JsonConvert.SerializeObject(new SampleConfiguration
                {
                    DataField1 = "value1",
                    DataField2 = "value2",
                    DataField3 = "value3",
                }),
                ExpiryDate = DateTime.Now.AddSeconds(60),
            };

            provider.Cache.TryAdd(SingleValueKey, expiredSingleValue);
            provider.Cache.TryAdd(SimpleObjectKey, simpleObjectCacheItem);
        }
    }
}