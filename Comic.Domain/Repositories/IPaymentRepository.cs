using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IPaymentRepository : IBaseRepository<Payments>
    {
        ValueTask UpdatepaymentOrder(List<int> paymentIds);
    }
}
