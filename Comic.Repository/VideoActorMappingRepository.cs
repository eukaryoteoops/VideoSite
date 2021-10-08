using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoActorMappingRepository : BaseRepository<VideoActorMapping>, IVideoActorMappingRepository
    {
        public VideoActorMappingRepository(IDbContext db) : base(db)
        {
        }
    }
}
