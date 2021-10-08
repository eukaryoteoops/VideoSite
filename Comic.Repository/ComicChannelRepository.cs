using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ComicChannelRepository : BaseRepository<ComicChannels>, IComicChannelRepository
    {
        public ComicChannelRepository(IDbContext db) : base(db)
        {
        }
    }
}
