using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.Cache.Interfaces;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Cache.Implements
{
    public class AdvertCache : ICache<Adverts>
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly CacheProvider _cacheProvider;

        public AdvertCache(CacheProvider cacheProvider, IAdvertRepository advertRepository)
        {
            _cacheProvider = cacheProvider;
            _advertRepository = advertRepository;
        }

        public async ValueTask<Adverts> GetOneAsync(string key, Expression<Func<Adverts, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<Adverts>(key);
            if (result == null)
            {
                result = await _advertRepository.GetOneAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<Adverts>(key, result, time);
            }
            return result;
        }

        public async ValueTask<IEnumerable<Adverts>> GetAsync(string key, Expression<Func<Adverts, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<IEnumerable<Adverts>>(key);
            if (result == null)
            {
                result = await _advertRepository.GetAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<IEnumerable<Adverts>>(key, result, time);
            }
            return result;
        }

        public async ValueTask ClearAsync(string key)
        {
            await _cacheProvider.ResetCache(key);
        }
    }
}
