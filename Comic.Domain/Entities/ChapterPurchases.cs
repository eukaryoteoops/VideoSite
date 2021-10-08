using System;

namespace Comic.Domain.Entities
{
    public class ChapterPurchases : Entity
    {
        public ChapterPurchases()
        {
        }

        public ChapterPurchases(int memberId, int comicId, int number)
        {
            MemberId = memberId;
            ComicId = comicId;
            ChapterNumber = number;
            CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public int MemberId { get; set; }
        public int ComicId { get; set; }
        public int ChapterNumber { get; set; }
        public long CreatedTime { get; set; }
    }
}
