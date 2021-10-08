namespace Comic.BackOffice.ReadModels.Member
{
    public class MemberRM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int MerchantId { get; set; }
        public string Source { get; set; }
        public bool IsVip { get; set; }
        public bool IsPremium { get; set; }
        public long LoginTime { get; set; }
        public long CreatedTime { get; set; }
        public int Point { get; set; }
        public bool State { get; set; }
    }
}
