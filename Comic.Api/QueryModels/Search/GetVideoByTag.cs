namespace Comic.Api.QueryModels.Search
{
    public class GetVideoByTag : Pagination
    {
        public string TagName { get; set; }
    }
}
