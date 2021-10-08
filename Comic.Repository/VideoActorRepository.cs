using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoActorRepository : BaseRepository<VideoActors>, IVideoActorRepository
    {
        public VideoActorRepository(IDbContext db) : base(db)
        {
        }
    }
}
