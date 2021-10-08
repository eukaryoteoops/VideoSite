namespace Comic.Api.ReadModels.Comic
{
    public class ComicBoxRM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ChapterCount { get; set; }
        public long UpdatedTime { get; set; }
    }
}
