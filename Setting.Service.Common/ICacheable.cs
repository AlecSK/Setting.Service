using System;

namespace Setting.Service.Common
{
    public interface ICacheable
    {
        DateTime? LoadedInCacheAt { get; set; }

    }
}
