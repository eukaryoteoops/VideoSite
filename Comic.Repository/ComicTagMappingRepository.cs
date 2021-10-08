using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ComicTagMappingRepository : BaseRepository<ComicTagMapping>, IComicTagMappingRepository
    {
        public ComicTagMappingRepository(IDbContext db) : base(db)
        {
        }
    }
}
