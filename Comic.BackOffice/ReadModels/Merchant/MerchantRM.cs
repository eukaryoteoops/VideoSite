namespace Comic.BackOffice.ReadModels.Merchant
{
    public class MerchantRM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public int Bonus { get; set; }
        public bool State { get; set; }
        public long CreatedTime { get; set; }
    }
}
