using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class CompensationJournalRepository : BaseRepository<CompensationJournals>, ICompensationJournalRepository
    {
        public CompensationJournalRepository(IDbContext db) : base(db)
        {
        }
    }
}
