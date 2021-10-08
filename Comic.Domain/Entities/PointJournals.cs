using System;

namespace Comic.Domain.Entities
{
    public class PointJournals : Entity
    {
        public PointJournals()
        {
        }

        public PointJournals(int id, int value, string remark)
        {
            MemberId = id;
            Value = value;
            Remark = remark;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public int MemberId { get; set; }
        public int Value { get; set; }
        public string Remark { get; set; }
        public long CreatedTime { get; set; }
    }
}
