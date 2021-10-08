using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IMerchantSubRepository : IBaseRepository<MerchantSubs>
    {
        ValueTask UpdateSubs(int parentId, List<int> childrenIds);
    }
}
