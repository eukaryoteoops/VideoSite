using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IPromoteUrlRepository : IBaseRepository<PromoteUrls>
    {
        ValueTask UpdatePromoteUrls(int id, List<string> urls);
    }
}
