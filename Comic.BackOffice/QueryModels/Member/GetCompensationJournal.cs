namespace Comic.BackOffice.QueryModels.Member
{
    public class GetCompensationJournal : Pagination
    {
        public string Name { get; set; }
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
    }
}
