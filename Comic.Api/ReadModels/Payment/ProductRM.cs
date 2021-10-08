namespace Comic.Api.ReadModels.Payment
{
    public class ProductRM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Price { get; set; }
        public int Value { get; set; }
        /// <summary>
        ///     1 : 點數
        ///     2 : 天數
        /// </summary>
        public byte Type { get; set; }
        public bool IsDefault { get; set; }
    }
}
