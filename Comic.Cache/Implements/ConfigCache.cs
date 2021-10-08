using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.Cache.Interfaces;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Cache.Implements
{
    public class ConfigCache : ICache<Configs>
    {
        private readonly IConfigRepository _configRepository;
        private readonly CacheProvider _cacheProvider;

        public ConfigCache(CacheProvider cacheProvider, IConfigRepository configRepository)
        {
            _cacheProvider = cacheProvider;
            _configRepository = configRepository;
        }

        public async ValueTask<Configs> GetOneAsync(string key, Expression<Func<Configs, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<Configs>(key);
            if (result == null)
            {
                result = await _configRepository.GetOneAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<Configs>(key, result, time);
            }
            return result;
        }

        public async ValueTask<IEnumerable<Configs>> GetAsync(string key, Expression<Func<Configs, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<IEnumerable<Configs>>(key);
            if (result == null)
            {
                result = await _configRepository.GetAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<IEnumerable<Configs>>(key, result, time);
            }
            return result;
        }

        public async ValueTask ClearAsync(string key)
        {
            await _cacheProvider.ResetCache(key);
        }
    }
}
