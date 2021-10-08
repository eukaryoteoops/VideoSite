namespace Comic.BackOffice.ReadModels.Comic
{
    public class ComicRM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte Channel { get; set; }
        public int ChapterCount { get; set; }
        public int TotalChapter { get; set; }
        public bool IsEnded { get; set; }
        public bool State { get; set; }
    }
}
