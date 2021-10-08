using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Comic.Domain.Repositories
{
    public interface IBaseRepository<T>
    {
        ValueTask<IEnumerable<T>> GetWithSortingAsync(
            Expression<Func<T, bool>> exp = null,
            string sort = null,
            int page = 1, int size = int.MaxValue);
        ValueTask<int> GetAmount(Expression<Func<T, bool>> exp = null);
        ValueTask<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> exp = null);
        ValueTask<T> GetOneAsync(Expression<Func<T, bool>> exp = null);
        ValueTask<T> AddAsync(T root);
        ValueTask UpdateAsync(T root);
        ValueTask DeleteAsync(T root);
    }
}
