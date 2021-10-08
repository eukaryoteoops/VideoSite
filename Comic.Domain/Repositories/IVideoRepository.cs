using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IVideoRepository : IBaseRepository<Videos>
    {
        ValueTask InsertVideo(Videos video);
    }
}
