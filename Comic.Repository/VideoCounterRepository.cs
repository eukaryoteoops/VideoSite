using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoCounterRepository : BaseRepository<VideoCounters>, IVideoCounterRepository
    {
        public VideoCounterRepository(IDbContext db) : base(db)
        {
        }
    }
}
