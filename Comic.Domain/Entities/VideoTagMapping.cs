using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class VideoTagMapping : Entity
    {
        public VideoTagMapping()
        {
        }

        public VideoTagMapping(string videoId, int tagId)
        {
            VideoId = videoId;
            TagId = tagId;
        }

        public string VideoId { get; set; }
        public int TagId { get; set; }
        [Navigation("TagId")]
        public VideoTags Tag { get; set; }
        [Navigation("VideoId")]
        public Videos Video { get; set; }


    }
}
