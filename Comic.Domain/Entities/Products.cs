namespace Comic.Domain.Entities
{
    public class Products : Entity
    {
        public Products(string name, int price, byte type, int value)
        {
            Name = name;
            Price = price;
            Type = type;
            Value = value;
            State = 1;
            Order = int.MaxValue;
        }

        public Products()
        {
        }

        public string Name { get; set; }
        public string Desc { get; set; }
        public int Price { get; set; }
        public byte Type { get; set; }
        public int Value { get; set; }
        public int Order { get; set; }
        public byte State { get; set; }

        public void UpdateState(byte state)
        {
            this.State = state;
        }

        public void ArchiveProdcut()
        {
            this.State = 2;
            this.Order = int.MaxValue;
        }
    }
}
