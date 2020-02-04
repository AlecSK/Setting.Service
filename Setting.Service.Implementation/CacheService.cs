using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Setting.Service.Common;
using Setting.Service.Contract.Interfaces;

namespace Setting.Service.Implementation
{

    public class CacheService<TItem> : ICacheService<TItem>, IDisposable where TItem : class, ICacheableList
    {
        private MemoryCache _cache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        public CacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public CacheService(MemoryCacheOptions memoryCacheOptions)
        {
            _cache = new MemoryCache(memoryCacheOptions);
        }


        public void ResetCache()
        {
            ResetCache(new MemoryCacheOptions());
        }

        public void ResetCache(MemoryCacheOptions memoryCacheOptions)
        {
            using (var cache = _cache)
            {
                Trace.WriteLine(" Old cache entries count = " + cache.Count);
                _cache = new MemoryCache(memoryCacheOptions);
            }
            Trace.WriteLine("New cache entries count = " + _cache.Count);
        }

        public int Count => _cache.Count;

        public bool TryGetCacheValue(object key, out TItem cacheEntry)
        {
            return _cache.TryGetValue(key, out cacheEntry);
        }


        public TItem GetAndStore(object key, Func<TItem> loadItemFunc)
        {
            TItem cacheEntry;
            if (loadItemFunc == null)
                throw new ArgumentNullException(nameof(loadItemFunc));

            var entryLock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

            entryLock.Wait();
            try
            {
                cacheEntry = loadItemFunc();
                cacheEntry.LoadedInCacheAt = DateTime.Now;
                _cache.Set(key, cacheEntry, new MemoryCacheEntryOptions
                {
                    Size = 1,
                    SlidingExpiration = TimeSpan.FromMinutes(cacheEntry.CacheEntryLifeTimeInMinutes)
                });
            }
            finally
            {
                entryLock.Release();
            }
            return cacheEntry;
        }


        public async Task<TItem> GetAndStoreAsync(object key, Func<Task<TItem>> loadItemFunc)
        {
            TItem cacheEntry;
            if (loadItemFunc == null)
                throw new ArgumentNullException(nameof(loadItemFunc));

            var entryLock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

            await entryLock.WaitAsync().ConfigureAwait(false);
            try
            {
                cacheEntry = await loadItemFunc().ConfigureAwait(false);
                cacheEntry.LoadedInCacheAt = DateTime.Now;
                _cache.Set(key, cacheEntry, new MemoryCacheEntryOptions
                {
                    Size = 1,
                    SlidingExpiration = TimeSpan.FromMinutes(cacheEntry.CacheEntryLifeTimeInMinutes)
                });
            }
            finally
            {
                entryLock.Release();
            }
            return cacheEntry;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                _cache?.Dispose();
            }
        }


    }

}