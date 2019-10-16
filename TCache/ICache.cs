using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TCache
{
    public interface ICache
    {
        ICacheProvider CacheProvider { get; }

        /// <summary>
        /// Define the default number of seconds to cache objects for.
        /// </summary>
        CacheDefaults DefaultCachePolicy { get; }

        void Add<T>(string key, T item, MemoryCacheEntryOptions policy);

        T Get<T>(string key);

        T GetOrAdd<T>(string key, Func<ICacheEntry, T> addItemFactory);

        Task<T> GetAsync<T>(string key);

        Task<T> GetOrAddAsync<T>(string key, Func<ICacheEntry, Task<T>> addItemFactory);

        void Remove(string key);

    }
}
