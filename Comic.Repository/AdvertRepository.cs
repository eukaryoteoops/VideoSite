using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class AdvertRepository : BaseRepository<Adverts>, IAdvertRepository
    {
        private readonly IDbContext _db;
        ~AdvertRepository()
        {
            _db.Dispose();
        }
        public AdvertRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdateAdvertOrder(List<int> advertIds)
        {
            var adverts = _db.Query<Adverts>(o => advertIds.Contains(o.Id)).ToList();
            var order = 1;
            foreach (var i in advertIds.Join(adverts, o => o, o => o.Id, (key, item) => item))
            {
                await _db.UpdateAsync<Adverts>(o => o.Id == i.Id, o => new Adverts() { Order = order });
                order++;
            }
        }
    }
}
