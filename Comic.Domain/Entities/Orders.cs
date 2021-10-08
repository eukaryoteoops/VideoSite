using System;
using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class Orders : Entity
    {
        public Orders()
        {

        }

        public Orders(int memberId, int paymentId, int productId, int merchantId, int merchantBonus)
        {
            OrderId = $"{DateTimeOffset.UtcNow.AddHours(8):yyyyMMdd}{new Random().Next(100000, 999999)}";
            MemberId = memberId;
            PaymentId = paymentId;
            ProductId = productId;
            MerchantId = merchantId;
            MerchantBonus = merchantBonus;
            State = false;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public string OrderId { get; set; }
        public int MemberId { get; set; }
        public int PaymentId { get; set; }
        public int ProductId { get; set; }
        public int MerchantId { get; set; }
        public int MerchantBonus { get; set; }
        public bool State { get; set; }
        public long CreatedTime { get; set; }
        public long? UpdatedTime { get; set; }
        [Navigation("MerchantId")]
        public Merchants Merchant { get; set; }
        [Navigation("PaymentId")]
        public Payments Payment { get; set; }
        [Navigation("ProductId")]
        public Products Product { get; set; }
        [Navigation("MemberId")]
        public Members Member { get; set; }

        public void UpdateState()
        {
            this.State = true;
            this.UpdatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }
    }
}
