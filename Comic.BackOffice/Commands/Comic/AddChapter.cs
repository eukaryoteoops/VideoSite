namespace Comic.BackOffice.Commands.Comic
{
    public class AddChapter
    {
        public int ComicId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public int Point { get; set; }
        public int Count { get; set; }
        public long EnabledTime { get; set; }
    }
}
