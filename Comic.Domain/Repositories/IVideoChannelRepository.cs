using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IVideoChannelRepository : IBaseRepository<VideoChannels>
    {
        ValueTask UpdateChannelOrder(List<int> channelIds);
    }
}
