using Dfc.SharedConfig.Cache;
using Dfc.SharedConfig.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using Xunit;

namespace Dfc.SharedConfig.UnitTests
{
    public class InMemorySharedConfigCacheProviderTests
    {
        private const string SingleValueKey = "singleValue";
        private const string SimpleObjectKey = "simpleObject";

        private readonly InMemorySharedConfigCacheProvider configCacheProvider;
        private readonly SharedConfigSettings settings;

        public InMemorySharedConfigCacheProviderTests()
        {
            var defaultCacheValues = GetDefaultCacheValues();

            settings = new SharedConfigSettings
            {
                ConfigurationStorageConnectionString = "DummyConnectionString",
                InMemoryCacheTimeToLiveTimeSpan = "00:01:00",
                CloudStorageTableName = "CloudTableName",
                EnvironmentName = "Develop"
            };

            configCacheProvider = new InMemorySharedConfigCacheProvider(settings, defaultCacheValues);
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
        public void SetConfigSavesDataToCache()
        {
            // Arrange
            const string newCacheData = "SomeData";
            var newCacheItemKey = Guid.NewGuid().ToString();

            // Act
            configCacheProvider.SetConfig(newCacheItemKey, newCacheData);

            // Assert
            var cacheResult = JsonConvert.DeserializeObject(configCacheProvider.GetConfig(newCacheItemKey));
            Assert.Equal(newCacheData, cacheResult);

            var cacheResultForDefaultValue = configCacheProvider.GetConfig(SimpleObjectKey);
            Assert.True(!string.IsNullOrEmpty(cacheResultForDefaultValue));
        }

        [Fact]
        public void SetConfigSavesDataToCacheWithoutDefaultValues()
        {
            // Arrange
            const string newCacheData = "OnlyItemOfData";
            var newCacheItemKey = Guid.NewGuid().ToString();
            var cacheProvider = new InMemorySharedConfigCacheProvider(settings);

            // Act
            cacheProvider.SetConfig(newCacheItemKey, newCacheData);

            // Assert
            var cacheResult = JsonConvert.DeserializeObject(cacheProvider.GetConfig(newCacheItemKey));
            Assert.Equal(newCacheData, cacheResult);

            var cacheResultForDefaultValue = JsonConvert.DeserializeObject(cacheProvider.GetConfig(SingleValueKey));
            Assert.Null(cacheResultForDefaultValue);
        }

        private static ConcurrentDictionary<string, object> GetDefaultCacheValues()
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

            var result = new ConcurrentDictionary<string, object>();
            result.TryAdd(SingleValueKey, expiredSingleValue);
            result.TryAdd(SimpleObjectKey, simpleObjectCacheItem);

            return result;
        }
    }
}