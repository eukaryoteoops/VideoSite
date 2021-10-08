using System.Collections.Generic;
using System.Linq;

namespace Comic.BackOffice.ReadModels.Merchant
{
    public class MerchantReportRM
    {
        public MerchantReportRM(IEnumerable<Detail> details, int pageNo, int pageSize)
        {
            Data = details;
            Details = details.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        public int Count => Data.Count();
        public int Amount => Data.Sum(o => o.ProductPrice);
        public decimal DeductPaymentAmount => Data.Sum(o => o.DeductPaymentAmount);
        public decimal AllotAmount => Data.Sum(o => o.AllotAmount);
        private IEnumerable<Detail> Data { get; set; }
        public IEnumerable<Detail> Details { get; set; }

        public class Detail
        {
            public int Id { get; set; }
            public string OrderId { get; set; }
            public string Name { get; set; }
            public long CreatedTime { get; set; }
            public string ProductName { get; set; }
            public int ProductPrice { get; set; }
            public decimal DeductPaymentAmount => (decimal)(ProductPrice * 0.9);
            public int MerchantBonus { get; set; }
            public decimal AllotAmount => DeductPaymentAmount * MerchantBonus / 100;
        }
    }
}
