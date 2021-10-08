using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ConfigRepository : BaseRepository<Configs>, IConfigRepository
    {
        public ConfigRepository(IDbContext db) : base(db)
        {
        }
    }
}
