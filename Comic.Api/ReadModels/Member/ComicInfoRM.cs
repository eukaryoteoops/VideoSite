using System;
using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.Api.ReadModels.Member
{
    public class ComicInfoRM
    {
        public ComicInfoRM(Members member, ComicFavorites favorite, ComicHistories reading, IEnumerable<ChapterPurchases> chapterPurchases)
        {
            IsPremium = member.IsPremium;
            IsVip = member.PackageTime > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            IsFavorite = favorite != null;
            LastChapter = reading?.Chapter ?? 0;
            Purchased = chapterPurchases.Select(o => o.ChapterNumber);
        }

        public bool IsPremium { get; set; }
        public bool IsVip { get; set; }
        public bool IsFavorite { get; }
        public int LastChapter { get; set; }
        public IEnumerable<int> Purchased { get; }
    }
}
