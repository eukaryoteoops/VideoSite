using System;
using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class ComicHistories : Entity
    {
        public ComicHistories()
        {
        }

        public ComicHistories(int memberId, int comicId, int chapter)
        {
            MemberId = memberId;
            ComicId = comicId;
            Chapter = chapter;
            ReadingTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public int MemberId { get; set; }
        public int ComicId { get; set; }
        public int Chapter { get; set; }
        public long ReadingTime { get; set; }
        [Navigation("ComicId")]
        public Comics Comic { get; set; }

        public void UpdateHistory(int chapter)
        {
            this.Chapter = chapter;
            this.ReadingTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }
    }
}
