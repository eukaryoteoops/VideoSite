namespace Comic.Api.QueryModels.Payment
{
    public class GetChannels
    {
        public int ProductId { get; set; }
        /// <summary>
        ///     1 : web
        ///     2 : mobile
        /// </summary>
        public byte Device { get; set; }
    }
}
