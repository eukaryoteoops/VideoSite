namespace Comic.Api.ReadModels.Member
{
    public class ChapterCountRM
    {
        public ChapterCountRM(int count, string title)
        {
            Count = count;
            Title = title;
        }

        public int Count { get; set; }
        public string Title { get; set; }
    }
}
