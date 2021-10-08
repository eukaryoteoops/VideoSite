namespace Comic.BackOffice.ReadModels.Comic
{
    public class ChapterRM
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public int Point { get; set; }
        public int Count { get; set; }
        public long EnabledTime { get; set; }
    }
}
