using System;
using System.Collections.Generic;
using System.Linq;
using Comic.Common.ExtensionMethods;
using Comic.Domain.Entities;

namespace Comic.BackOffice.Merchant.ReadModels.Home
{
    public class StatisticRM
    {
        public StatisticRM(IEnumerable<Members> members, IEnumerable<Orders> orders)
        {
            Members = members;
            Orders = orders;
            var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

            Daily = Aggregate(
               now.Date.WithOffset(8).ToUnixTimeSeconds(),
                now.Date.AddDays(1).WithOffset(8).ToUnixTimeSeconds());
            Monthly = Aggregate(
                new DateTime(now.Year, now.Month, 1).WithOffset(8).ToUnixTimeSeconds(),
                new DateTime(now.Year, now.Month, 1).AddMonths(1).WithOffset(8).ToUnixTimeSeconds());
            Total = Aggregate(long.MinValue, long.MaxValue);
        }

        public Statistic Daily { get; set; }
        public Statistic Monthly { get; set; }
        public Statistic Total { get; set; }
        private IEnumerable<Members> Members { get; }
        private IEnumerable<Orders> Orders { get; }

        private Statistic Aggregate(long startTime, long endTime)
        {
            return new Statistic
            {
                MemberCount = Members.Where(o => o.CreatedTime >= startTime && o.CreatedTime < endTime).Count(),
                OrderCount = Orders.Where(o => o.CreatedTime >= startTime && o.CreatedTime < endTime).Count(),
                Amount = Orders.Where(o => o.CreatedTime >= startTime && o.CreatedTime < endTime).Sum(o => o.Product.Price),
                AllotAmount = Orders.Where(o => o.CreatedTime >= startTime && o.CreatedTime < endTime).Sum(o => (decimal)(o.Product.Price * 0.9 * o.MerchantBonus / 100))
            };
        }

        public class Statistic
        {
            public int MemberCount { get; set; }
            public int OrderCount { get; set; }
            public int Amount { get; set; }
            public decimal AllotAmount { get; set; }
        }
    }
}
