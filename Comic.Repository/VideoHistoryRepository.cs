using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoHistoryRepository : BaseRepository<VideoHistories>, IVideoHistoryRepository
    {
        public VideoHistoryRepository(IDbContext db) : base(db)
        {
        }
    }
}
