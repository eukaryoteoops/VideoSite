using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class ComicStatisticRepository : BaseRepository<ComicStatistics>, IComicStatisticRepository
    {
        private readonly IDbContext _db;
        public ComicStatisticRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdateStatistics(List<ComicStatistics> counters, List<ComicStatistics> favorites)
        {
            _db.Session.BeginTransaction();
            try
            {
                var stats = _db.Query<ComicStatistics>().ToList();
                foreach (var counter in counters)
                {
                    if (stats.Any(o => o.Type == 1 && o.ComicId == counter.ComicId))
                        _db.Update<ComicStatistics>(o => o.Type == 1 && o.ComicId == counter.ComicId, o => new ComicStatistics { Count = o.Count + counter.Count });
                    else
                        _db.Insert(counter);
                }
                foreach (var fav in favorites)
                {
                    if (stats.Any(o => o.Type == 2 && o.ComicId == fav.ComicId))
                        _db.Update<ComicStatistics>(o => o.Type == 2 && o.ComicId == fav.ComicId, o => new ComicStatistics { Count = o.Count + fav.Count });
                    else
                        _db.Insert(fav);
                }
            }
            catch (Exception ex)
            {
                _db.Session.RollbackTransaction();
                throw new Exception($"UpdateComicStatistics Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
