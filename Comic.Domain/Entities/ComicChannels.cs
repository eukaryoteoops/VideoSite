namespace Comic.Domain.Entities
{
    public class ComicChannels : Entity
    {
        public ComicChannels()
        {
        }

        public string Name { get; set; }
        public bool State { get; set; }

        public void UpdateState(bool state)
        {
            this.State = state;
        }
    }

    public enum ComicChannelEnum
    {
        韓漫 = 1,
        同人誌 = 2
    }
}
