namespace Comic.BackOffice.ReadModels.Product
{
    public class ProductRM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Price { get; set; }
        public byte Type { get; set; }
        public int Value { get; set; }
        public int Order { get; set; }
        public bool State { get; set; }
    }
}
