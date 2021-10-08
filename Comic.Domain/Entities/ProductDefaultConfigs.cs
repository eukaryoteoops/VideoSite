using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class ProductDefaultConfigs : Entity
    {

        public ProductDefaultConfigs(byte type, int id)
        {
            Type = type;
            Id = id;
        }

        public ProductDefaultConfigs()
        {
        }
        public byte Type { get; set; }
        public int ProductId { get; set; }
        [Navigation("ProductId")]
        public Products Product { get; set; }


        public void UpdateProductDefault(int id)
        {
            this.ProductId = id;
        }
    }
}
