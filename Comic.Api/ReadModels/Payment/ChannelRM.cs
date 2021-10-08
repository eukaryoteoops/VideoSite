namespace Comic.Api.ReadModels.Payment
{
    public class ChannelRM
    {
        /// <summary>
        ///     1 : 支付寶
        ///     2 : 微信
        /// </summary>
        public byte Type { get; set; }
        public string ChannelName { get; set; }
        public string DisplayName { get; set; }
        public string Color { get; set; }
        public int Id { get; set; }
    }
}
