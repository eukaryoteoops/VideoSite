namespace Comic.BackOffice.Commands.Comic
{
    public class UpdateChapter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Point { get; set; }
        public int Count { get; set; }
        public long EnabledTime { get; set; }
    }
}
