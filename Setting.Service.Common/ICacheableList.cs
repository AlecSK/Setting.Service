namespace Setting.Service.Common
{
    public interface ICacheableList : ICacheable
    {
        int CacheEntryLifeTimeInMinutes { get; set; }
    }
}
