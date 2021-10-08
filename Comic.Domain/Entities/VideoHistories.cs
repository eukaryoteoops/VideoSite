using System;
using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class VideoHistories : Entity
    {
        public VideoHistories()
        {
        }

        public VideoHistories(int memberId, string cid)
        {
            MemberId = memberId;
            Cid = cid;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public int MemberId { get; set; }
        public string Cid { get; set; }
        public long CreatedTime { get; set; }
        [Navigation("Cid")]
        public Videos Video { get; set; }

        public void UpdateHistory()
        {
            this.CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }
    }
}
