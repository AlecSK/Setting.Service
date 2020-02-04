namespace Setting.Service.Contract.Interfaces
{
    public interface IAppSettings
    {

        int DefaultCacheLifeTimeInMinutes { get; set; }

        int MaxSettingCountInCache { get; set; }
    }
}
