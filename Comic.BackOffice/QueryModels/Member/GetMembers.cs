namespace Comic.BackOffice.QueryModels.Member
{
    public class GetMembers : Pagination
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? MerchantId { get; set; }
        public string Source { get; set; }
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
        /// <summary>
        ///     0 : all
        ///     1 : vip
        ///     2 : premium
        /// </summary>
        public byte VipType { get; set; }
    }
}
