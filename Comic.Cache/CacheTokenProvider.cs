using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Comic.Cache
{
    public class CacheTokenProvider
    {
        private IDatabase _cache;
        private readonly string _connStr;

        public CacheTokenProvider(IConfiguration conf)
        {
            _connStr = conf.GetSection("Connection:RedisToken").Value;
        }

        public string GetStringAndRefresh(string key, TimeSpan time)
        {
            Connect();
            var cachedResponse = _cache.StringGet(key);
            if (!cachedResponse.IsNull)
            {
                _cache.KeyExpire(key, time);
                return cachedResponse.ToString();
            }
            return null;
        }

        public string GetString(string key)
        {
            Connect();
            var cachedResponse = _cache.StringGet(key);
            return cachedResponse.IsNull ? null : cachedResponse.ToString();
        }

        public async ValueTask<string> GetStringAsync(string key)
        {
            Connect();
            var cachedResponse = await _cache.StringGetAsync(key);
            return cachedResponse.IsNull ? null : cachedResponse.ToString();
        }

        public async ValueTask<T> GetAsync<T>(string key) where T : class
        {
            Connect();
            var cachedResponse = await _cache.StringGetAsync(key);
            return cachedResponse.IsNull ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async ValueTask SetStringAsync(string key, string result, TimeSpan? time = null)
        {
            Connect();
            await _cache.StringSetAsync(key, result, time ?? TimeSpan.FromMinutes(5));
        }

        public async ValueTask SetAsync<T>(string key, T result, TimeSpan? time = null) where T : class
        {
            Connect();
            await _cache.StringSetAsync(key, JsonSerializer.Serialize(result), time ?? TimeSpan.FromMinutes(5));
        }

        public async ValueTask ResetCache(string key)
        {
            Connect();
            await _cache.KeyDeleteAsync(key);
        }

        private void Connect()
        {
            if (_cache != null) return;
            var conn = ConnectionMultiplexer.Connect(_connStr);
            _cache = conn.GetDatabase();
        }
    }
}
