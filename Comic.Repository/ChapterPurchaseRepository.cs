using System;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ChapterPurchaseRepository : BaseRepository<ChapterPurchases>, IChapterPurchaseRepository
    {
        private readonly IDbContext _db;

        public ChapterPurchaseRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask PurchaseChapter(Members member, Chapters chapter)
        {
            _db.Session.BeginTransaction();
            try
            {
                member.UpdatePoint(chapter.Point * -1);
                await _db.UpdateAsync(member);
                var purchase = new ChapterPurchases(member.Id, chapter.ComicId, chapter.Number);
                await _db.InsertAsync(purchase);
                var journal = new PointJournals(member.Id, chapter.Point * -1, $"{chapter.ComicId}-{chapter.Number}");
                await _db.InsertAsync(journal);
            }
            catch (Exception)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("PurchaseChapter Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
