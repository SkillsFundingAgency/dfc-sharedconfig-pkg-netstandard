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
        internal readonly ConcurrentDictionary<string, object> Cache = new ConcurrentDictionary<string, object>();
        private readonly SharedConfigSettings settings;

        public InMemorySharedConfigCacheProvider(SharedConfigSettings settings)
        {
            this.settings = settings;
        }

        public string GetConfig(string key)
        {
            if (!this.Cache.TryGetValue(key, out var cachedValue))
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

            this.Cache.TryAdd(key, cacheItem);
        }
    }
}