using System;
using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class ComicFavorites : Entity
    {
        public ComicFavorites()
        {
        }

        public ComicFavorites(int memberId, int comicId)
        {
            MemberId = memberId;
            ComicId = comicId;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public int MemberId { get; set; }
        public int ComicId { get; set; }
        public long CreatedTime { get; set; }
        [Navigation("ComicId")]
        public Comics Comic { get; set; }
    }
}
