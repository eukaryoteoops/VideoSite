using System;

namespace Comic.BackOffice.ReadModels.Merchant
{
    public class PlatformReportRM
    {
        public DateTimeOffset Date { get; set; }
        public int MemberCount { get; set; }
        public int OrderCount { get; set; }
        public int Amount { get; set; }
    }
}
