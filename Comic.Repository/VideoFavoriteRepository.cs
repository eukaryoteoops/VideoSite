using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoFavoriteRepository : BaseRepository<VideoFavorites>, IVideoFavoriteRepository
    {
        public VideoFavoriteRepository(IDbContext db) : base(db)
        {
        }
    }
}
