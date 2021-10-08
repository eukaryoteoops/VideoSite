using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class OrderRepository : BaseRepository<Orders>, IOrderRepository
    {
        private readonly IDbContext _db;
        ~OrderRepository()
        {
            _db.Dispose();
        }
        public OrderRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask<List<int>> GetRePurchaseMembers()
        {
            return _db.Query<Orders>(o => o.State).GroupBy(o => o.MemberId).Having(o => Sql.Count() > 1).Select(o => o.MemberId).ToList();
        }

        public async ValueTask OrderSuccess(Orders order)
        {
            _db.Session.BeginTransaction();
            try
            {
                // 1. change order state 
                order.UpdateState();
                await _db.UpdateAsync(order);
                // 2. update point/packagetime
                var member = await _db.Query<Members>(o => o.Id == order.MemberId).FirstAsync();
                switch (order.Product.Type)
                {
                    case 1:
                        var pointJournal = new PointJournals(member.Id, order.Product.Value, "點數方案");
                        await _db.InsertAsync(pointJournal);
                        member.UpdatePoint(order.Product.Value);
                        await _db.UpdateAsync(member); break;
                    case 2:
                        if (order.Product.Value == 9999) member.SetPremium();
                        else member.UpdatePackage(order.Product.Value);
                        await _db.UpdateAsync(member); break;
                    default:
                        throw new Exception("order.Product.Type error.");
                }
            }
            catch (Exception ex)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("OrderSuccess Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
