
using NatechWeather.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace NatechWeather.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private static readonly TimeSpan defaultExpiration = new TimeSpan(36000000000);

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return Task.FromResult(_memoryCache.Get<T>(key));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? defaultExpiration
            };

            _memoryCache.Set(key, value, cacheEntryOptions);
            return Task.CompletedTask;
        }
    }
}
