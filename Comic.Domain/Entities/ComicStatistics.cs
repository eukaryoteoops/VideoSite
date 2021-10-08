using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class ComicStatistics : Entity
    {
        public ComicStatistics()
        {
        }

        public ComicStatistics(byte type, int comicId, int count)
        {
            Type = type;
            ComicId = comicId;
            Count = count;
        }

        public byte Type { get; set; }
        public int ComicId { get; set; }
        public int Count { get; set; }
        [Navigation("ComicId")]
        public Comics Comic { get; set; }
    }
}
