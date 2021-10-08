namespace Comic.BackOffice.ReadModels.Payment
{
    public class PaymentRM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ChannelName { get; set; }
        public string DisplayName { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public int Order { get; set; }
        /// <summary>
        ///     1 : web
        ///     2 : mobile
        /// </summary>
        public byte Device { get; set; }
        public bool State { get; set; }
    }
}
