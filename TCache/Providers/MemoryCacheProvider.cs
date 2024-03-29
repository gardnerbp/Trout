﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace TCache.Providers
{
    public class MemoryCacheProvider : ICacheProvider
    {
        internal readonly IMemoryCache cache;

        public MemoryCacheProvider(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public object GetOrCreate<T>(string key, Func<ICacheEntry, T> factory)
        {
            return cache.GetOrCreate(key, factory);
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<ICacheEntry, Task<T>> factory)
        {
            return cache.GetOrCreateAsync(key, factory);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public void Set(string key, object item, MemoryCacheEntryOptions policy)
        {
            cache.Set(key, item, policy);
        }

        public void Dispose()
        {
            cache?.Dispose();
        }
    }
}
