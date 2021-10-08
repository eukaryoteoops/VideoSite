namespace Comic.BackOffice.QueryModels.Order
{
    public class GetOrders : Pagination
    {
        public string Name { get; set; }
        public string ChannelName { get; set; }
        public bool? State { get; set; }
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
    }
}
