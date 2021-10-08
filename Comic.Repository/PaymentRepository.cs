using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class PaymentRepository : BaseRepository<Payments>, IPaymentRepository
    {
        private readonly IDbContext _db;
        ~PaymentRepository()
        {
            _db.Dispose();
        }
        public PaymentRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdatepaymentOrder(List<int> paymentIds)
        {
            var payments = _db.Query<Payments>(o => paymentIds.Contains(o.Id)).ToList();
            var order = 1;
            foreach (var i in paymentIds.Join(payments, o => o, o => o.Id, (key, item) => item))
            {
                await _db.UpdateAsync<Payments>(o => o.Id == i.Id, o => new Payments() { Order = order });
                order++;
            }
        }
    }
}
