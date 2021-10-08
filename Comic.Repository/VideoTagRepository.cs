using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoTagRepository : BaseRepository<VideoTags>, IVideoTagRepository
    {
        public VideoTagRepository(IDbContext db) : base(db)
        {
        }
    }
}
