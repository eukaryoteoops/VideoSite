using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.BackOffice.ReadModels.Home
{
    public class PurchaseStatisticRM
    {
        public PurchaseStatisticRM(string date, List<Orders> orders, List<int> repurchaseIds)
        {
            Date = date;

            FirstVipDaily = orders.Count(o => o.ProductId == 1 && !repurchaseIds.Contains(o.MemberId));
            FirstVipMonthly = orders.Count(o => o.ProductId == 2 && !repurchaseIds.Contains(o.MemberId));
            FirstVipQuarterly = orders.Count(o => o.ProductId == 3 && !repurchaseIds.Contains(o.MemberId));
            FirstVipAnnually = orders.Count(o => o.ProductId == 4 && !repurchaseIds.Contains(o.MemberId));
            FirstVipPremium = orders.Count(o => o.ProductId == 9 && !repurchaseIds.Contains(o.MemberId));
            FirstPoint5000 = orders.Count(o => o.ProductId == 5 && !repurchaseIds.Contains(o.MemberId));
            FirstPoint13000 = orders.Count(o => o.ProductId == 6 && !repurchaseIds.Contains(o.MemberId));
            FirstPoint24000 = orders.Count(o => o.ProductId == 7 && !repurchaseIds.Contains(o.MemberId));
            FirstPoint38000 = orders.Count(o => o.ProductId == 8 && !repurchaseIds.Contains(o.MemberId));

            ReVipDaily = orders.Count(o => o.ProductId == 1 && repurchaseIds.Contains(o.MemberId));
            ReVipMonthly = orders.Count(o => o.ProductId == 2 && repurchaseIds.Contains(o.MemberId));
            ReVipQuarterly = orders.Count(o => o.ProductId == 3 && repurchaseIds.Contains(o.MemberId));
            ReVipAnnually = orders.Count(o => o.ProductId == 4 && repurchaseIds.Contains(o.MemberId));
            ReVipPremium = orders.Count(o => o.ProductId == 9 && repurchaseIds.Contains(o.MemberId));
            RePoint5000 = orders.Count(o => o.ProductId == 5 && repurchaseIds.Contains(o.MemberId));
            RePoint13000 = orders.Count(o => o.ProductId == 6 && repurchaseIds.Contains(o.MemberId));
            RePoint24000 = orders.Count(o => o.ProductId == 7 && repurchaseIds.Contains(o.MemberId));
            RePoint38000 = orders.Count(o => o.ProductId == 8 && repurchaseIds.Contains(o.MemberId));
        }
        public string Date { get; set; }
        public int FirstVipDaily { get; set; }
        public int FirstVipMonthly { get; set; }
        public int FirstVipQuarterly { get; set; }
        public int FirstVipAnnually { get; set; }
        public int FirstVipPremium { get; set; }
        public int FirstPoint5000 { get; set; }
        public int FirstPoint13000 { get; set; }
        public int FirstPoint24000 { get; set; }
        public int FirstPoint38000 { get; set; }

        public int ReVipDaily { get; set; }
        public int ReVipMonthly { get; set; }
        public int ReVipQuarterly { get; set; }
        public int ReVipAnnually { get; set; }
        public int ReVipPremium { get; set; }
        public int RePoint5000 { get; set; }
        public int RePoint13000 { get; set; }
        public int RePoint24000 { get; set; }
        public int RePoint38000 { get; set; }
    }
}
