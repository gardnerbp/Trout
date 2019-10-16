using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TCache.Providers;

namespace TCache
{
    // +------------------------------------------------------------------------------+
    // |                                                                              |
    // |     Trout is developed by Dave Gardner.  Copyright 2019.                     |
    // |     Trout is free software.  It is distributed under the GNU General         |
    // |     Public License Version 3 (GPLv3).  See LICENSE file for details.         |
    // |     See https://github.com/gardnerbp/Trout for user and developer guides.    |
    // |                                                                              |
    // +------------------------------------------------------------------------------+

    public class Cache : ICache
    {
        // Provinence:
        // https://michaelscodingspot.com/cache-implementations-in-csharp-net/
        // https://www.codeproject.com/Articles/7684/Using-C-Generics-to-implement-a-Cache-collection
        // https://github.com/alastairtree/LazyCache
        // https://www.codeproject.com/Articles/290935/Using-MemoryCache-in-Net-4-0

        // http://cachemanager.michaco.net/
        // https://codeshare.co.uk/blog/simple-reusable-net-caching-example-code-in-c/
        // https://cpratt.co/thread-safe-strongly-typed-memory-caching-c-sharp/

        private readonly Lazy<ICacheProvider> cacheProvider;

        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        #region Constructors
        public Cache() : this(DefaultCacheProvider) { }      // If no CacheProvider is provided, use the default one.

        public Cache(Lazy<ICacheProvider> cacheProvider)
        {
            this.cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public Cache(Func<ICacheProvider> cacheProviderFactory)
        {
            if (cacheProviderFactory == null) throw new ArgumentNullException(nameof(cacheProviderFactory));
            cacheProvider = new Lazy<ICacheProvider>(cacheProviderFactory);
        }

        #endregion

        /// <summary>
        /// The default cache provider is an in-memory cache from Microsoft.Extensions.Caching.Memory
        /// </summary>
        public static Lazy<ICacheProvider> DefaultCacheProvider { get; set; }
            = new Lazy<ICacheProvider>(() =>
                new MemoryCacheProvider( new MemoryCache( new CacheDefaults().CacheOptions()))
        );

        public virtual ICacheProvider CacheProvider => cacheProvider.Value;

        /// <summary>
        /// Policy defining how long items should be cached for unless specified
        /// </summary>
        public virtual CacheDefaults DefaultCachePolicy { get; set; } = new CacheDefaults();

        #region ICacheImplementation
        public void Add<T>(string key, T item, MemoryCacheEntryOptions policy)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            ValidateKey(key);

            CacheProvider.Set(key, item, policy);
        }

        public T Get<T>(string key)
        {
            ValidateKey(key);
            var item = CacheProvider.Get(key);
            return GetValueFromLazy<T>(item);
        }

        public Task<T> GetAsync<T>(string key)
        {
            ValidateKey(key);
            var item = CacheProvider.Get(key);
            return GetValueFromAsyncLazy<T>(item);
        }

        public T GetOrAdd<T>(string key, Func<ICacheEntry, T> addItemFactory)
        {
            ValidateKey(key);

            object cacheItem;
            locker.Wait(); //TODO: do we really need this? Could we just lock on the key?
            try
            {
                cacheItem = CacheProvider.GetOrCreate<object>(key, entry =>
                    new Lazy<T>(() =>
                    {
                        var result = addItemFactory(entry);
                        EnsureEvictionCallbackDoesNotReturnTheAsyncOrLazy<T>(entry.PostEvictionCallbacks);
                        return result;
                    })
                );
            }
            finally
            {
                locker.Release();
            }

            try
            {
                return GetValueFromLazy<T>(cacheItem);
            }
            catch //addItemFactory errored so do not cache the exception
            {
                CacheProvider.Remove(key);
                throw;
            }
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<ICacheEntry, Task<T>> addItemFactory)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            ValidateKey(key);
            CacheProvider.Remove(key);
        }
        #endregion


        protected virtual T GetValueFromLazy<T>(object item)
        {
            switch (item)
            {
                case Lazy<T> lazy:
                    return lazy.Value;
                case T variable:
                    return variable;
                case AsyncLazy<T> asyncLazy:
                    // this is async to sync - and should not really happen as long as GetOrAddAsync is used for an async
                    // value. Only happens when you cache something async and then try and grab it again later using
                    // the non async methods.
                    return asyncLazy.Value.ConfigureAwait(false).GetAwaiter().GetResult();
                case Task<T> task:
                    return task.Result;
            }

            return default(T);
        }

        protected virtual Task<T> GetValueFromAsyncLazy<T>(object item)
        {
            switch (item)
            {
                case AsyncLazy<T> asyncLazy:
                    return asyncLazy.Value;
                case Task<T> task:
                    return task;
                // this is sync to async and only happens if you cache something sync and then get it later async
                case Lazy<T> lazy:
                    return Task.FromResult(lazy.Value);
                case T variable:
                    return Task.FromResult(variable);
            }

            return Task.FromResult(default(T));
        }

        protected virtual void EnsureEvictionCallbackDoesNotReturnTheAsyncOrLazy<T>(
            IList<PostEvictionCallbackRegistration> callbackRegistrations)
        {
            if (callbackRegistrations != null)
                foreach (var item in callbackRegistrations)
                {
                    var originalCallback = item.EvictionCallback;
                    item.EvictionCallback = (key, value, reason, state) =>
                    {
                        // before the original callback we need to unwrap the Lazy that holds the cache item
                        if (value is AsyncLazy<T> asyncCacheItem)
                            value = asyncCacheItem.IsValueCreated ? asyncCacheItem.Value : Task.FromResult(default(T));
                        else if (value is Lazy<T> cacheItem)
                            value = cacheItem.IsValueCreated ? cacheItem.Value : default(T);

                        // pass the unwrapped cached value to the original callback
                        originalCallback(key, value, reason, state);
                    };
                }
        }

        protected virtual void ValidateKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException(nameof(key), "Cache keys cannot be empty or whitespace");
        }

    }
}
