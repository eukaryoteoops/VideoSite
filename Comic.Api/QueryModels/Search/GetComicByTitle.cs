namespace Comic.Api.QueryModels.Search
{
    public class GetComicByTitle : Pagination
    {
        public string Title { get; set; }
    }
}
