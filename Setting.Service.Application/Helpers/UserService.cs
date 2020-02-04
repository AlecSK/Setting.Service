using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Setting.Service.Application.Interfaces;
using Setting.Service.Application.Models;
using Setting.Service.Common;
using Setting.Service.Contract;
using Setting.Service.Contract.Interfaces;
using Setting.Service.DataAccess;
using Setting.Service.Implementation;

namespace Setting.Service.Application.Helpers
{
    public class UserService : IUserService
    {

        private readonly ILogger<UserService> _logger;
        private readonly IAppSettings _appSettings;
        private readonly MasterDataDbContext _db;

        private static readonly ICacheService<CacheableList<User>> UsersCache = new CacheService<CacheableList<User>>();

        public UserService(ILoggerFactory loggerFactory, IAppSettings appSettings, MasterDataDbContext db)
        {
            _logger = loggerFactory.CreateLogger<UserService>();
            _appSettings = appSettings;
            _db = db;
            _logger.LogInformation("Сервис инициализирован.");
        }

        private IEnumerable<User> GetUsersFromDb()
        {
            var setting = _db.Settings.Single(t =>
                t.ConfigurationModule.Name == AppConstants.SettingServiceModuleName && t.Name == AppConstants.UserCredentials).Value;
            var credentials = setting.Split(';');
            foreach (var credential in credentials)
            {
                var userData = credential.Split(':');
                yield return new User { Username = userData[0], Password = userData[1], LoadedInCacheAt = DateTime.Now };
            }
        }


        private List<User> GetUsersWithCache()
        {
            if (UsersCache.TryGetCacheValue(0, out var cachedUsers))
                if (!cachedUsers.IsExpired())
                {
                    _logger.LogDebug("{0}   Список пользователей загружен из кэша.", "GetUsersWithCache");
                    return cachedUsers;
                }

            var users = UsersCache.GetAndStore(0, () =>
            {
                return new CacheableList<User>(GetUsersFromDb(), _appSettings.DefaultCacheLifeTimeInMinutes);
            });
            _logger.LogDebug("{0}   Список пользователей загружен из БД.", "GetUsersWithCache");

            return users;
        }


        public async Task<User> Authenticate(string username, string password)
        {
            var users = GetUsersWithCache();

            var user = await Task.Run(() => users.SingleOrDefault(x => x.Username == username && x.Password == password)).ConfigureAwait(false);

            return user?.WithoutPassword();  // user returns from cache by ref
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var users = GetUsersWithCache();

            return await Task.Run(() => users.WithoutPasswords()).ConfigureAwait(false);
        }
    }
}