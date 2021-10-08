using System;
using System.Collections.Generic;
using System.Linq;
using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class Videos
    {
        public Videos()
        {
        }

        public Videos(string cid, string channel, string name, string intro, string videoUrl, string imageUrl, int enabledDate, string tagString, string actorString)
        {
            Cid = cid;
            Enum.TryParse(channel, out ChannelEnum result);
            Channel = result;
            Name = name.Trim('\n');
            Introduction = intro.Trim('\n');
            VideoUrl = videoUrl;
            ImageUrl = imageUrl;
            EnabledDate = enabledDate;
            Tags = tagString.Split('|').Where(o => !string.IsNullOrEmpty(o)).Select(o => new VideoTags(o.Trim('\n'))).ToList();
            Actors = actorString.Split('|').Where(o => !string.IsNullOrEmpty(o)).Select(o => new VideoActors(o.Trim('\n'))).ToList();
            State = true;
            Point = 1;
        }
        [Column(IsPrimaryKey = true)]
        public string Cid { get; set; }
        public ChannelEnum Channel { get; set; }
        public string Name { get; set; }
        public string Introduction { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public int EnabledDate { get; set; }
        public bool State { get; set; }
        public int Point { get; set; }
        [NotMapped]
        public IEnumerable<VideoTags> Tags { get; set; }
        [NotMapped]
        public IEnumerable<VideoActors> Actors { get; set; }

        public void UpdatePoint(int point)
        {
            this.Point = point;
        }

        public void UpdateState(bool state)
        {
            this.State = state;
        }
    }
}
