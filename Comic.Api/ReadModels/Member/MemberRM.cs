namespace Comic.Api.ReadModels.Member
{
    public class MemberRM
    {
        public string Name { get; set; }
        public int Point { get; set; }
        public long PackageTime { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsPremium { get; set; }
        public bool IsPurchased { get; set; }
    }
}
