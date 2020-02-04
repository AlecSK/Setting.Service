using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Setting.Service.Common;

namespace Setting.Service.Contract.Interfaces
{
    public interface ICacheService<TItem> where TItem : class, ICacheableList
    {
        void ResetCache();
        void ResetCache(MemoryCacheOptions memoryCacheOptions);

        int Count { get; }

        bool TryGetCacheValue(object key, out TItem cacheEntry);

        TItem GetAndStore(object key, Func<TItem> loadItemFunc);

        Task<TItem> GetAndStoreAsync(object key, Func<Task<TItem>> loadItemFunc);

    }
}
