using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.BackOffice.ReadModels.Video
{
    public class VideoDetailRM
    {
        public VideoDetailRM() { }

        public string Cid { get; set; }
        public ChannelEnum Channel { get; set; }
        public string Name { get; set; }
        public string Introduction { get; set; }
        public int EnabledDate { get; set; }
        public bool State { get; set; }
        /// <summary>
        ///     0 : 免費
        ///     0以外 : VIP
        /// </summary>
        public int Point { get; set; }
        public int Views { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Actors { get; set; }

        internal void AddInfo(IEnumerable<VideoTagMapping> tags, IEnumerable<VideoActorMapping> actors, int views)
        {
            this.Tags = tags?.Select(o => o.Tag.Name);
            this.Actors = actors?.Select(o => o.Actor.Name);
            this.Views = views;
        }
    }
}
