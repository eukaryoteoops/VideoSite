namespace Comic.BackOffice.Merchant.QueryModels.Report
{
    public class GetPerformanceReport : Pagination
    {
        public long StartTime { get; set; }
        public long EndTime { get; set; }
    }
}
