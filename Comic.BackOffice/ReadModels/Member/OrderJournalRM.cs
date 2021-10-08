namespace Comic.BackOffice.ReadModels.Member
{
    public class OrderJournalRM
    {
        public string PaymentName { get; set; }
        public string ChannelName { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public long CreatedTime { get; set; }
    }
}
