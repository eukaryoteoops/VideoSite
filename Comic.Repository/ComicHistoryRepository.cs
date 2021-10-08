using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ComicHistoryRepository : BaseRepository<ComicHistories>, IComicHistoryRepository
    {
        public ComicHistoryRepository(IDbContext db) : base(db)
        {
        }
    }
}
