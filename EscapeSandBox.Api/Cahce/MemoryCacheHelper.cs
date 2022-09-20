using Microsoft.Extensions.Caching.Memory;

namespace EscapeSandBox.Api.Cahce
{
    public class MemoryCacheHelper
    {
        //创建MemoryCache对象
        public static IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 取缓存项，如果不存在则返回空。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            try
            {
                return cache.Get<T>(key);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 取缓存项，如果不存在则新增缓存项。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cachePopulate"></param>
        /// <param name="slidingExpiration"></param>
        /// <returns></returns>
        public static T GetOrAddCacheItem<T>(string key, int slidingExpiration, Func<T> func)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Invalid cache key");

            if (cache.TryGetValue(key, out T result))
            {
                return result;
            }

            var newValue = func();

            if (newValue != null)
            {
                result = cache.Set(key, newValue, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingExpiration)));
            }

            return result;
        }

        /// <summary>
        /// 移除指定键的缓存项
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            cache.Remove(key);
        }
    }

}
