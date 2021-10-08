using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.Api.ReadModels.Video
{
    public class VideoInfoRM
    {
        public VideoInfoRM(Videos video, IEnumerable<VideoTags> tags, IEnumerable<VideoActors> actors, IEnumerable<VideoStatistics> statistics)
        {
            Name = video.Name;
            Channel = (int)video.Channel;
            Point = video.Point;
            Tags = tags.Select(o => o.Name);
            Actors = actors.Select(o => o.Name);
            ClickCount = statistics.FirstOrDefault(o => o.Type == 1)?.Count ?? 0;
            FavoriteCount = statistics.FirstOrDefault(o => o.Type == 2)?.Count ?? 0;
        }

        public string Name { get; set; }
        public int Channel { get; set; }
        public int Point { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Actors { get; set; }
        public int ClickCount { get; set; }
        public int FavoriteCount { get; set; }
    }
}
