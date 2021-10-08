using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ComicCounterRepository : BaseRepository<ComicCounters>, IComicCounterRepository
    {
        public ComicCounterRepository(IDbContext db) : base(db)
        {
        }
    }
}
