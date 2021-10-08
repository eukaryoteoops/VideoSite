using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class PointJournalRepository : BaseRepository<PointJournals>, IPointJournalRepository
    {
        public PointJournalRepository(IDbContext db) : base(db)
        {
        }
    }
}
