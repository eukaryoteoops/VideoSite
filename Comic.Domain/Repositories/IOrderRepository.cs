using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IOrderRepository : IBaseRepository<Orders>
    {
        ValueTask OrderSuccess(Orders order);
        ValueTask<List<int>> GetRePurchaseMembers();
    }
}
