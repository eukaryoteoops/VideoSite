using Microsoft.AspNetCore.Mvc;

namespace Comic.Api.QueryModels.Video
{
    public class GetByChannel : Pagination
    {
        [FromRoute]
        public int Channel { get; set; }
    }
}
