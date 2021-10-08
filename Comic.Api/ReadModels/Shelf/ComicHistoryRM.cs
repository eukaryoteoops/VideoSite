namespace Comic.Api.ReadModels.Shelf
{
    public class ComicHistoryRM
    {
        public int Id { get; set; }
        public int ComicId { get; set; }
        public string ComicTitle { get; set; }
        public int Chapter { get; set; }
        public long ReadingTime { get; set; }

    }
}
