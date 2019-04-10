using System;
using System.Collections.Generic;
using System.Text;
using EasyCaching.Core;

namespace stats_eba_bot.Cache
{
    public class SqlLiteCacheService
    {
        private readonly IEasyCachingProviderFactory _factory;

        private string _cacheProvider => "QCSqlLite";
        public SqlLiteCacheService(IEasyCachingProviderFactory factory)
        {
            _factory = factory;
        }

        public void SaveInCache(string key, string value)
        {
            var provider = _factory.GetCachingProvider(_cacheProvider);    
            provider.Set(key,value, TimeSpan.FromDays(60));
        }

        public string GetFromCache(string key)
        {
            var provider = _factory.GetCachingProvider(_cacheProvider);    
            var value = provider.Get<string>(key);
            if (value.HasValue)
            {
                return value.Value;
            }

            return string.Empty;
        }
    }
}
