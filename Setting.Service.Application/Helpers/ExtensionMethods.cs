using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Setting.Service.Application.Models;

namespace Setting.Service.Application.Helpers
{
    public static class ExtensionMethods
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorLoggingMiddleware>();
        }

        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users) {
            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            if (user == null) 
                return null;

            var other = user.ShallowCopy();
            other.Password = null;
            return other;

        }
    }
}