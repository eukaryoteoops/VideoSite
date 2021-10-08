using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class BaseRepository<T> : IBaseRepository<T>
    {
        private readonly IDbContext _db;

        public BaseRepository(IDbContext db)
        {
            _db = db;
        }

        public async ValueTask<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> exp = null)
        {
            var result = await _db.Query<T>().IncludeAll().Where(exp ?? (_ => true)).ToListAsync();
            return result;
        }

        public async ValueTask<T> GetOneAsync(Expression<Func<T, bool>> exp = null)
        {
            var result = await _db.Query<T>().IncludeAll().Where(exp ?? (_ => true)).FirstOrDefaultAsync();
            return result;
        }

        public async ValueTask<T> AddAsync(T root)
        {
            var result = await _db.InsertAsync(root);
            return result;
        }

        public async ValueTask UpdateAsync(T root)
        {
            await _db.UpdateAsync(root);
        }

        public async ValueTask DeleteAsync(T root)
        {
            await _db.DeleteAsync(root);
        }

        public async ValueTask<IEnumerable<T>> GetWithSortingAsync(Expression<Func<T, bool>> exp = null, string sort = null, int page = 1, int size = int.MaxValue)
        {
            var data = _db.Query<T>().IncludeAll().Where(exp ?? (_ => true));
            if (sort != null)
                data = data.OrderBy(sort);
            var result = await data.TakePage(page, size).ToListAsync();
            return result;
        }

        public async ValueTask<int> GetAmount(Expression<Func<T, bool>> exp = null)
        {
            var result = await _db.Query<T>().IncludeAll().Where(exp ?? (_ => true)).CountAsync();
            return result;
        }
    }
}
