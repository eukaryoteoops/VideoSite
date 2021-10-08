using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.Cache.Interfaces;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Cache.Implements
{
    public class OrderCache : ICache<Orders>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly CacheProvider _cacheProvider;

        public OrderCache(CacheProvider cacheProvider, IOrderRepository orderRepository)
        {
            _cacheProvider = cacheProvider;
            _orderRepository = orderRepository;
        }

        public async ValueTask<Orders> GetOneAsync(string key, Expression<Func<Orders, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<Orders>(key);
            if (result == null)
            {
                result = await _orderRepository.GetOneAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<Orders>(key, result, time);
            }
            return result;
        }

        public async ValueTask<IEnumerable<Orders>> GetAsync(string key, Expression<Func<Orders, bool>> exp = null, TimeSpan? time = null)
        {
            var result = await _cacheProvider.GetAsync<IEnumerable<Orders>>(key);
            if (result == null)
            {
                result = await _orderRepository.GetAsync(exp ?? (_ => true));
                await _cacheProvider.SetAsync<IEnumerable<Orders>>(key, result, time);
            }
            return result;
        }

        public async ValueTask ClearAsync(string key)
        {
            await _cacheProvider.ResetCache(key);
        }
    }
}
