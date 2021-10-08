using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.Cache.Interfaces;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Cache.Implements
{
    public class MemberCache : ICache<Members>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly CacheProvider _cacheProvider;

        public MemberCache(IMemberRepository memberRepository, CacheProvider cacheProvider)
        {
            _memberRepository = memberRepository;
            _cacheProvider = cacheProvider;
        }

        public async ValueTask<Members> GetOneAsync(string key, Expression<Func<Members, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<Members>(key);
            if (result == null)
            {
                result = await _memberRepository.GetOneAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<Members>(key, result, time);
            }
            return result;
        }

        public async ValueTask<IEnumerable<Members>> GetAsync(string key, Expression<Func<Members, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<IEnumerable<Members>>(key);
            if (result == null)
            {
                result = await _memberRepository.GetAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<IEnumerable<Members>>(key, result, time);
            }
            return result;
        }

        public async ValueTask ClearAsync(string key)
        {
            await _cacheProvider.ResetCache(key);
        }
    }
}
