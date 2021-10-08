using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IChapterRepository : IBaseRepository<Chapters>
    {
        ValueTask AddChapter(Chapters chapter);
        ValueTask UpdateFreeComics(List<int> freeIds, List<int> changeIds);
    }
}
