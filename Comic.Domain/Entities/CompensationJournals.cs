using System;
using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class CompensationJournals : Entity
    {
        public CompensationJournals()
        {
        }

        public CompensationJournals(int id, byte type, int value, int managerId)
        {
            MemberId = id;
            Type = type;
            Value = value;
            ManagerId = managerId;
            CreatedTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
        }

        public int MemberId { get; set; }
        public byte Type { get; set; }
        public int Value { get; set; }
        public int ManagerId { get; set; }
        public long CreatedTime { get; set; }
        [Navigation("MemberId")]
        public Members Member { get; set; }
        [Navigation("ManagerId")]
        public Managers Manager { get; set; }
    }
}
