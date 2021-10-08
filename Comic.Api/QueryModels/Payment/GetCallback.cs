namespace Comic.Api.QueryModels.Payment
{
    public class GetCallback
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
        public string Sign { get; set; }
    }
}
