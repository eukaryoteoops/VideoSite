namespace Comic.Domain.Entities
{
    public class VideoActors : Entity
    {
        public VideoActors()
        {
        }

        public VideoActors(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
