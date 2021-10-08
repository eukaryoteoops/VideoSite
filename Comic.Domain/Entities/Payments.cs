namespace Comic.Domain.Entities
{
    public class Payments : Entity
    {
        public string Name { get; set; }
        public byte Type { get; set; }
        public string ChannelName { get; set; }
        public string DisplayName { get; set; }
        public string Color { get; set; }
        public int Code { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public int Order { get; set; }
        public byte Device { get; set; }
        public bool State { get; set; }

        public void UpdateState(bool state)
        {
            this.State = state;
        }
    }
}
