using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ProductRepository : BaseRepository<Products>, IProductRepository
    {
        private readonly IDbContext _db;
        ~ProductRepository()
        {
            _db.Dispose();
        }
        public ProductRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdateProductOrder(List<int> productIds)
        {
            var products = _db.Query<Products>(o => productIds.Contains(o.Id)).ToList();
            var order = 1;
            foreach (var i in productIds.Join(products, o => o, o => o.Id, (key, item) => item))
            {
                await _db.UpdateAsync<Products>(o => o.Id == i.Id, o => new Products() { Order = order });
                order++;
            }
        }
    }
}
