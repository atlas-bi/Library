using Atlas_Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Helpers
{
    public static class UserHelpers
    {
        public static List<UserPreference> GetPreferences(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username ??= "default";
            return cache.GetOrCreate<List<UserPreference>>(
                "Preferences-" + username,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context
                        .UserPreferences.Where(x => x.User.Username == username)
                        .ToList();
                }
            );
        }
    }
}
