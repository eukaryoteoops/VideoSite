using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IAdvertRepository : IBaseRepository<Adverts>
    {
        ValueTask UpdateAdvertOrder(List<int> advertIds);
    }
}
