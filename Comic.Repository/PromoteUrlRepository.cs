using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class PromoteUrlRepository : BaseRepository<PromoteUrls>, IPromoteUrlRepository
    {
        private readonly IDbContext _db;
        public PromoteUrlRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdatePromoteUrls(int id, List<string> urls)
        {
            _db.Session.BeginTransaction();
            try
            {
                await _db.DeleteAsync<PromoteUrls>(o => o.MerchantId == id);
                var newUrls = new List<PromoteUrls>();
                urls.ForEach(o => newUrls.Add(new PromoteUrls(id, o)));
                await _db.InsertRangeAsync(newUrls);
            }
            catch (Exception ex)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("UpdatePromoteUrls Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
