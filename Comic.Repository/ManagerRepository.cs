using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ManagerRepository : BaseRepository<Managers>, IManagerRepository
    {
        public ManagerRepository(IDbContext db) : base(db)
        {
        }
    }
}
