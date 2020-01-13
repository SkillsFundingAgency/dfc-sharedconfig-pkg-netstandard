using System;

namespace Dfc.SharedConfig.Models
{
    internal class InMemoryCacheItem
    {
        internal string Data { get; set; }

        internal DateTime ExpiryDate { get; set; }
    }
}