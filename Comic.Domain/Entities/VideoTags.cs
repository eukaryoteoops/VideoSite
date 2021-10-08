namespace Comic.Domain.Entities
{
    public class VideoTags : Entity
    {
        public VideoTags()
        {
        }

        public VideoTags(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
