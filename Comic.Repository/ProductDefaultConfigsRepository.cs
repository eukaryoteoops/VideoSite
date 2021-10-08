using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ProductDefaultConfigsRepository : BaseRepository<ProductDefaultConfigs>, IProductDefaultConfigsRepository
    {
        public ProductDefaultConfigsRepository(IDbContext db) : base(db)
        {
        }
    }
}
