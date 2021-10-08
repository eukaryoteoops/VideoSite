using System;
using Comic.Domain.Entities;

namespace Comic.Api.ReadModels.Member
{
    public class VideoInfoRM
    {
        public VideoInfoRM(Members member, VideoFavorites favorite)
        {
            IsPremium = member.IsPremium;
            IsVip = member.PackageTime > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            IsFavorite = favorite != null;
        }

        public bool IsPremium { get; set; }
        public bool IsVip { get; set; }
        public bool IsFavorite { get; }
    }
}
