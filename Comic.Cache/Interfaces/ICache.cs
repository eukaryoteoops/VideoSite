using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Comic.Cache.Interfaces
{
    public interface ICache<T> where T : class
    {
        ValueTask<T> GetOneAsync(string key, Expression<Func<T, bool>> exp = null, TimeSpan? time = null);
        ValueTask<IEnumerable<T>> GetAsync(string key, Expression<Func<T, bool>> exp = null, TimeSpan? time = null);
        ValueTask ClearAsync(string key);
    }
}
