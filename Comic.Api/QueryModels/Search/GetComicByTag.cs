namespace Comic.Api.QueryModels.Search
{
    public class GetComicByTag : Pagination
    {
        public string TagName { get; set; }
    }
}
