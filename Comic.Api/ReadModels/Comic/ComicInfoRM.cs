using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.Api.ReadModels.Comic
{
    public class ComicInfoRM
    {
        public ComicInfoRM(Comics comic, IEnumerable<Chapters> chapters, IEnumerable<ComicTags> tags, IEnumerable<ComicStatistics> statistics)
        {
            Title = comic.Title;
            Author = comic.Author;
            Desc = comic.Desc;
            UpdatedTime = comic.UpdatedTime;
            IsEnded = comic.IsEnded;
            ClickCount = statistics.FirstOrDefault(o => o.Type == 1)?.Count ?? 0;
            FavoriteCount = statistics.FirstOrDefault(o => o.Type == 2)?.Count ?? 0;
            Tags = tags;
            Chapters = chapters.Select(o => new ChapterRM { Number = o.Number, Title = o.Title, Point = o.Point, EnabledTime = o.EnabledTime });
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Desc { get; set; }
        public long? UpdatedTime { get; set; }
        public bool IsEnded { get; set; }
        public int ClickCount { get; set; }
        public int FavoriteCount { get; set; }
        public IEnumerable<ComicTags> Tags { get; set; }
        public IEnumerable<ChapterRM> Chapters { get; }
        public class ChapterRM
        {
            public int Number { get; set; }
            public string Title { get; set; }
            public int Point { get; set; }
            public long EnabledTime { get; set; }
        }
    }
}
