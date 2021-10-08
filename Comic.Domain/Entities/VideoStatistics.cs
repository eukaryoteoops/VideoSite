using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class VideoStatistics : Entity
    {
        public VideoStatistics()
        {
        }

        public VideoStatistics(byte type, string cid, int count)
        {
            Type = type;
            Cid = cid;
            Count = count;
        }

        public byte Type { get; set; }
        public string Cid { get; set; }
        public int Count { get; set; }
        [Navigation("Cid")]
        public Videos Video { get; set; }
    }
}
