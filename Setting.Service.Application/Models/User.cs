using System;
using Setting.Service.Common;
using Setting.Service.Contract.Interfaces;

namespace Setting.Service.Application.Models
{
    public class User : ICacheable
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? LoadedInCacheAt { get; set; }

        public User ShallowCopy()
        {
            return (User) this.MemberwiseClone();
        }
    }
}