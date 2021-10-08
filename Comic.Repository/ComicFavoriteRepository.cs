using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ComicFavoriteRepository : BaseRepository<ComicFavorites>, IComicFavoriteRepository
    {
        public ComicFavoriteRepository(IDbContext db) : base(db)
        {
        }
    }
}
