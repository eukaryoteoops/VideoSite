using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.BackOffice.Merchant.ReadModels.Report
{
    public class MerchantSubReportRM
    {
        public MerchantSubReportRM() { }
        public MerchantSubReportRM(int id, string name, IEnumerable<Orders> orders)
        {
            Id = id;
            Name = name;
            OrderCount = orders.Count();
            Amount = orders.Sum(o => o.Product.Price);
            DeductPaymentAmount = (decimal)orders.Sum(o => o.Product.Price * 0.9);
            AllotAmount = (decimal)orders.Sum(o => o.Product.Price * 0.9 * o.MerchantBonus / 100);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
        public int OrderCount { get; set; }
        public int Amount { get; set; }
        public decimal DeductPaymentAmount { get; set; }
        public decimal AllotAmount { get; set; }

    }
}
