using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class ComicTagMapping : Entity
    {
        public ComicTagMapping()
        {
        }

        public ComicTagMapping(int comicId, int tagId)
        {
            ComicId = comicId;
            TagId = tagId;
        }

        public int ComicId { get; set; }
        public int TagId { get; set; }
        [Navigation("TagId")]
        public ComicTags Tag { get; set; }
        [Navigation("ComicId")]
        public Comics Comic { get; set; }


    }
}
