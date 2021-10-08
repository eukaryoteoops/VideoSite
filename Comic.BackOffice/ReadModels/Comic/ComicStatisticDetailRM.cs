namespace Comic.BackOffice.ReadModels.Comic
{
    public class ComicStatisticDetailRM
    {
        public ComicStatisticDetailRM(int id, string title, int count)
        {
            ComicId = id;
            Title = title;
            Count = count;
        }

        public int ComicId { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
    }
}
