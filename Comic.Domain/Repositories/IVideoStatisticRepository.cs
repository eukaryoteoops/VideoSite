using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IVideoStatisticRepository : IBaseRepository<VideoStatistics>
    {
        ValueTask UpdateStatistics(List<VideoStatistics> counters, List<VideoStatistics> favorites);
    }
}
