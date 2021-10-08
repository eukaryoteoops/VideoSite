namespace Comic.Domain.Entities
{
    public class PromoteUrls : Entity
    {
        public PromoteUrls()
        {
        }

        public PromoteUrls(int merchantId, string url)
        {
            MerchantId = merchantId;
            Url = url;
        }

        public int MerchantId { get; set; }
        public string Url { get; set; }
    }
}
