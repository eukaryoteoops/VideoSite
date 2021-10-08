using System;

namespace Comic.Domain.Entities
{
    public class Comics : Entity
    {
        public Comics()
        {
        }

        public Comics(string title, byte channel, string author, string desc, bool isEnded)
        {
            Title = title;
            Channel = (ComicChannelEnum)channel;
            Author = author;
            Desc = desc;
            ChapterCount = 0;
            TotalChapter = 0;
            IsEnded = isEnded;
            State = true;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public string Title { get; set; }
        public ComicChannelEnum Channel { get; set; }
        public string Author { get; set; }
        public string Desc { get; set; }
        public int ChapterCount { get; set; }
        public int TotalChapter { get; set; }
        public bool IsEnded { get; set; }
        public bool State { get; set; }
        public long CreatedTime { get; set; }
        public long? UpdatedTime { get; set; }

        public void UpdateComic(string title, byte channel, string author, string desc, bool isEnded)
        {
            this.Title = string.IsNullOrEmpty(title) ? this.Title : title;
            this.Channel = (ComicChannelEnum)channel;
            this.Author = string.IsNullOrEmpty(author) ? this.Author : author;
            this.Desc = string.IsNullOrEmpty(desc) ? this.Desc : desc;
            this.IsEnded = isEnded;
        }

        public void UpdateState(bool state)
        {
            this.State = state;
        }
    }
}
