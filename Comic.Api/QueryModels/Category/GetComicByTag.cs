namespace Comic.Api.QueryModels.Category
{
    public class GetComicByTag : Pagination
    {
        public string TagName { get; set; }
        public bool? IsEnded { get; set; }
    }
}
