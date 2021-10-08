using Comic.Domain.Entities;

namespace Comic.BackOffice.QueryModels.Video
{
    public class GetVideos : Pagination
    {
        public string Cid { get; set; }
        public string Name { get; set; }
        public ChannelEnum? Channel { get; set; }
        public bool? State { get; set; }
        public int? StartDate { get; set; }
        public int? EndDate { get; set; }
    }
}
