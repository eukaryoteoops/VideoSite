using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ComicRepository : BaseRepository<Comics>, IComicRepository
    {
        private readonly IDbContext _db;
        ~ComicRepository()
        {
            _db.Dispose();
        }
        public ComicRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask AddComicAsync(Comics comic, List<int> tags)
        {
            _db.Session.BeginTransaction();
            try
            {
                var result = await _db.InsertAsync(comic);
                var comicId = Convert.ToInt32(result.Id);
                var mapping = new List<ComicTagMapping>();
                foreach (var item in tags)
                    mapping.Add(new ComicTagMapping(comicId, item));
                await _db.InsertRangeAsync(mapping);
            }
            catch (Exception)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("AddComic Error.");
            }
            _db.Session.CommitTransaction();
        }

        public async ValueTask UpdateComic(Comics comic, List<int> tags)
        {
            _db.Session.BeginTransaction();
            try
            {
                var result = await _db.UpdateAsync(comic);
                await _db.DeleteAsync<ComicTagMapping>(o => o.ComicId == comic.Id);
                var mapping = new List<ComicTagMapping>();
                foreach (var item in tags)
                    mapping.Add(new ComicTagMapping(comic.Id, item));
                await _db.InsertRangeAsync(mapping);
            }
            catch (Exception)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("UpdateComic Error.");
            }
            _db.Session.CommitTransaction();
        }

        public async ValueTask<int> AddComicTempAsync(Comics comic, List<int> tags)
        {
            var id = 0;
            _db.Session.BeginTransaction();
            try
            {
                var result = await _db.InsertAsync(comic);
                var comicId = Convert.ToInt32(result.Id);
                id = result.Id;
                var mapping = new List<ComicTagMapping>();
                foreach (var item in tags)
                    mapping.Add(new ComicTagMapping(comicId, item));
                await _db.InsertRangeAsync(mapping);
            }
            catch (Exception)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("AddComic Error.");
            }
            _db.Session.CommitTransaction();
            return id;
        }

        public async ValueTask UpdateDailyComics(IEnumerable<Chapters> chapters)
        {
            _db.Session.BeginTransaction();
            try
            {
                foreach (var item in chapters)
                {
                    await _db.UpdateAsync<Comics>(o => o.Id == item.ComicId, o => new Comics
                    {
                        ChapterCount = item.Number,
                        UpdatedTime = item.EnabledTime
                    });
                }
            }
            catch (Exception)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("UpdateDailyComics Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
