namespace Comic.Api.ReadModels.Member
{
    public class MemberPointJournalRM
    {
        public long CreatedTime { get; set; }
        /// <summary>
        ///     1 : 購買章節
        ///     2 : 系統補點
        ///     3 : 充值點數
        /// </summary>
        public byte Action { get; set; }
        public int Value { get; set; }
    }
}
