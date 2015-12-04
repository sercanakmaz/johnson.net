using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JohnsonNet.Operation
{
    public class CacheOperation
    {
        public void Set<T>(string key, T val, TimeSpan? expire = null)
        {

        }
        public T Get<T>(string key, T defaultValue = default(T), TimeSpan? expire = null, Func<T> getAction = null)
        {
            if (!expire.HasValue) expire = TimeSpan.FromMinutes(30);

            if (HttpContext.Current == null) return JohnsonManager.Convert.Default(getAction(), defaultValue);

            var cache = HttpContext.Current.Cache;

            var result = cache[key] as CacheValueItem<T>;

            if (result == null ? true : DateTime.Now > result.Expire)
            {
                cache[key] = result = new CacheValueItem<T>
                {
                    Object = getAction(),
                    Expire = DateTime.Now.Add(expire.Value)
                };
            }

            return result == null ? defaultValue : JohnsonManager.Convert.Default(result.Object, defaultValue);
        }
        class CacheValueItem<T>
        {
            public DateTime Expire { get; set; }
            public T Object { get; set; }
        }
    }
}
