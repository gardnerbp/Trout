using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCache
{
    public class CacheDefaults
    {
        public virtual int DefaultCacheDurationSeconds { get; set; } = 60 * 20;

        internal MemoryCacheEntryOptions BuildOptions()
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(DefaultCacheDurationSeconds)
            };
        }

        internal MemoryCacheOptions CacheOptions()
        {
            return new MemoryCacheOptions {  SizeLimit=4096 };
        }


    }
}
