using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ChapterRepository : BaseRepository<Chapters>, IChapterRepository
    {
        private readonly IDbContext _db;

        public ChapterRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask AddChapter(Chapters chapter)
        {
            _db.Session.BeginTransaction();
            try
            {
                await _db.InsertAsync(chapter);
                await _db.UpdateAsync<Comics>(o => o.Id == chapter.ComicId, o => new Comics { TotalChapter = o.TotalChapter + 1 });
            }
            catch (Exception)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("AddChapter Error.");
            }
            _db.Session.CommitTransaction();
        }

        public async ValueTask UpdateFreeComics(List<int> freeIds, List<int> changeIds)
        {
            _db.Session.BeginTransaction();
            try
            {
                await _db.UpdateAsync<Chapters>(o => freeIds.Contains(o.Id), o => new Chapters { Point = 60 });
                await _db.UpdateAsync<Chapters>(o => changeIds.Contains(o.Id), o => new Chapters { Point = 0 });
            }
            catch (Exception)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("UpdateFreeComics Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
