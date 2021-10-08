using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.BackOffice.ReadModels.Merchant
{
    public class PlatformReportDeatilRM
    {
        public PlatformReportDeatilRM(int merchantId, List<Orders> orders, List<Members> members)
        {
            MerchantId = merchantId;
            MemberWebCount = members.Where(o => o.Source != "android" && o.Source != "ios").Count();
            MemberiOsCount = members.Where(o => o.Source == "ios").Count();
            MemberAndroidCount = members.Where(o => o.Source == "android").Count();
            OrderCount = orders.Count();
            Amount = orders.Sum(o => o.Product.Price);
            DeductPaymentAmount = (decimal)orders.Sum(o => o.Product.Price * 0.9);
            AllotAmount = (decimal)orders.Sum(o => o.Product.Price * 0.9 * o.MerchantBonus / 100);
        }
        public int MerchantId { get; set; }
        public int MemberWebCount { get; set; }
        public int MemberiOsCount { get; set; }
        public int MemberAndroidCount { get; set; }

        public int OrderCount { get; set; }
        public int Amount { get; set; }
        public decimal DeductPaymentAmount { get; set; }
        public decimal AllotAmount { get; set; }
    }
}
