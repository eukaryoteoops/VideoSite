namespace Comic.BackOffice.ReadModels.Order
{
    public class OrderRM
    {
        public string OrderId { get; set; }
        public string MemberName { get; set; }
        public string PaymentName { get; set; }
        public string ChannelName { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int MerchantId { get; set; }
        public bool State { get; set; }
        public long CreatedTime { get; set; }
        public long UpdatedTime { get; set; }
    }
}
