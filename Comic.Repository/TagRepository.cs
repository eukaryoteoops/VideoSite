using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class TagRepository : BaseRepository<ComicTags>, ITagRepository
    {
        public TagRepository(IDbContext db) : base(db)
        {
        }
    }
}
