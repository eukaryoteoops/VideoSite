using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IComicStatisticRepository : IBaseRepository<ComicStatistics>
    {
        ValueTask UpdateStatistics(List<ComicStatistics> counters, List<ComicStatistics> favorites);
    }
}
