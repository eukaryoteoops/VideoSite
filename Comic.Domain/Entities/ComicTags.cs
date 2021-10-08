namespace Comic.Domain.Entities
{
    public class ComicTags : Entity
    {
        public ComicTags()
        {
        }

        public ComicTags(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
