namespace Comic.Api.QueryModels.Payment
{
    public class GetPaymentUrl
    {
        public int ProductId { get; set; }
        public int PaymentId { get; set; }
        public string ReturnUrl { get; set; }
    }
}
