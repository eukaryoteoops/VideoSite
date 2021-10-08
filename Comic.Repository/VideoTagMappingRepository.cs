using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoTagMappingRepository : BaseRepository<VideoTagMapping>, IVideoTagMappingRepository
    {
        public VideoTagMappingRepository(IDbContext db) : base(db)
        {
        }
    }
}
