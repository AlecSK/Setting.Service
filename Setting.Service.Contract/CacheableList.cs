using System;
using System.Collections.Generic;
using Setting.Service.Common;
using Setting.Service.Contract.Interfaces;

namespace Setting.Service.Contract
{
    public class CacheableList<T> : List<T>, ICacheableList where T : class, ICacheable
    {
        public CacheableList(IEnumerable<T> items, int lifeTimeInMinutes) : base(items)
        {
            CacheEntryLifeTimeInMinutes = lifeTimeInMinutes;
            foreach (T item in this)
            {
                item.LoadedInCacheAt = DateTime.Now;
            }
            LoadedInCacheAt = DateTime.Now;
        }

        public DateTime? LoadedInCacheAt { get; set; }

        public int CacheEntryLifeTimeInMinutes { get; set; }

        public bool IsExpired()
        {
            if (!LoadedInCacheAt.HasValue)
                return true;

            if (CacheEntryLifeTimeInMinutes == 0)
                return true;

            var diff = (DateTime.Now - LoadedInCacheAt.Value).Minutes;
            return diff > CacheEntryLifeTimeInMinutes;
        }

    }
}
