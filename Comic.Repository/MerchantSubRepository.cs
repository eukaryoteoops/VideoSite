using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class MerchantSubRepository : BaseRepository<MerchantSubs>, IMerchantSubRepository
    {
        private readonly IDbContext _db;
        public MerchantSubRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdateSubs(int parentId, List<int> childrenIds)
        {
            _db.Session.BeginTransaction();
            try
            {
                await _db.DeleteAsync<MerchantSubs>(o => o.ParentId == parentId);
                var newSubs = new List<MerchantSubs>();
                childrenIds.ForEach(o => newSubs.Add(new MerchantSubs(parentId, o)));
                await _db.InsertRangeAsync(newSubs);
            }
            catch (Exception ex)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("UpdateSubs Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
