using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class Chapters : Entity
    {
        public Chapters()
        {
        }

        public Chapters(int comicId, int number, string title, int point, int count, long enabledTime)
        {
            ComicId = comicId;
            Number = number;
            Title = title;
            Point = point;
            Count = count;
            EnabledTime = enabledTime;
        }

        public int ComicId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public int Point { get; set; }
        public int Count { get; set; }
        public long EnabledTime { get; set; }
        [Navigation("ComicId")]
        public Comics Comic { get; set; }

        public void UpdateChapter(string title, int point, int count, long enabledTime)
        {
            this.Title = string.IsNullOrEmpty(title) ? this.Title : title;
            this.Point = point;
            this.Count = count == 0 ? this.Count : count;
            this.EnabledTime = enabledTime == 0 ? this.EnabledTime : enabledTime;


        }
    }
}
