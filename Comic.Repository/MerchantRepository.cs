using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class MerchantRepository : BaseRepository<Merchants>, IMerchantRepository
    {
        public MerchantRepository(IDbContext db) : base(db)
        {
        }
    }
}
