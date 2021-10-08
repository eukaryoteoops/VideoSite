namespace Comic.BackOffice.QueryModels.Merchant
{
    public class GetMerchantReport : Pagination
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
    }
}
