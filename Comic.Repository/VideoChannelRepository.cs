using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoChannelRepository : BaseRepository<VideoChannels>, IVideoChannelRepository
    {
        private readonly IDbContext _db;
        public VideoChannelRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask UpdateChannelOrder(List<int> channelIds)
        {
            var products = _db.Query<VideoChannels>(o => channelIds.Contains(o.Id)).ToList();
            var order = 1;
            foreach (var i in channelIds.Join(products, o => o, o => o.Id, (key, item) => item))
            {
                await _db.UpdateAsync<VideoChannels>(o => o.Id == i.Id, o => new VideoChannels() { Order = order });
                order++;
            }
        }
    }
}
