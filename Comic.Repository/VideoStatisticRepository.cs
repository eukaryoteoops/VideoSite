using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoStatisticRepository : BaseRepository<VideoStatistics>, IVideoStatisticRepository
    {
        private readonly IDbContext _db;
        public VideoStatisticRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdateStatistics(List<VideoStatistics> counters, List<VideoStatistics> favorites)
        {
            _db.Session.BeginTransaction();
            try
            {
                _db.Delete<VideoStatistics>(o => o.Type == 1 || o.Type == 2);
                await _db.InsertRangeAsync(counters);
                await _db.InsertRangeAsync(favorites);
            }
            catch (Exception ex)
            {
                _db.Session.RollbackTransaction();
                throw new Exception($"UpdateVideoStatistics Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
