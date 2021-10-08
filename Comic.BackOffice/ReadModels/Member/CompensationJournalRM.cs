namespace Comic.BackOffice.ReadModels.Member
{
    public class CompensationJournalRM
    {
        public string Name { get; set; }
        public byte Type { get; set; }
        public int Value { get; set; }
        public string ManagerName { get; set; }
        public long CreatedTime { get; set; }
    }
}
