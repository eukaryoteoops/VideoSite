using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IComicRepository : IBaseRepository<Comics>
    {
        ValueTask AddComicAsync(Comics comic, List<int> tags);
        ValueTask UpdateComic(Comics comic, List<int> tags);
        ValueTask<int> AddComicTempAsync(Comics comic, List<int> tags);
        ValueTask UpdateDailyComics(IEnumerable<Chapters> chapters);
    }
}
