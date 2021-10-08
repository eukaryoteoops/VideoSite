using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IProductRepository : IBaseRepository<Products>
    {
        ValueTask UpdateProductOrder(List<int> productIds);
    }
}
