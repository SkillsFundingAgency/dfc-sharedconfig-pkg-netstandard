using Dfc.SharedConfig.Contracts;
using Dfc.SharedConfig.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Dfc.SharedConfig.UnitTests")]

namespace Dfc.SharedConfig.Cache
{
    internal class InMemorySharedConfigCacheProvider : ISharedConfigCacheProvider
    {
        private readonly SharedConfigSettings settings;
        private readonly ConcurrentDictionary<string, object> cache;

        public InMemorySharedConfigCacheProvider(SharedConfigSettings settings)
        {
            this.settings = settings;
            this.cache = new ConcurrentDictionary<string, object>();
        }

        public InMemorySharedConfigCacheProvider(SharedConfigSettings settings, ConcurrentDictionary<string, object> cacheValues)
        {
            this.settings = settings;
            this.cache = cacheValues;
        }

        public string GetConfig(string key)
        {
            if (!this.cache.TryGetValue(key, out var cachedValue))
            {
                return string.Empty;
            }

            var inMemoryCacheItem = cachedValue as InMemoryCacheItem;
            return inMemoryCacheItem?.ExpiryDate < DateTime.UtcNow ? string.Empty : inMemoryCacheItem?.Data;
        }

        public void SetConfig(string key, object value)
        {
            var expiryDatetime = DateTime.UtcNow;
            if (TimeSpan.TryParse(this.settings.InMemoryCacheTimeToLiveTimeSpan, out var parsedTimespan))
            {
                expiryDatetime = expiryDatetime.Add(parsedTimespan);
            }

            var cacheItem = new InMemoryCacheItem
            {
                Data = JsonConvert.SerializeObject(value),
                ExpiryDate = expiryDatetime,
            };

            this.cache.TryAdd(key, cacheItem);
        }
    }
}