namespace Comic.Api.QueryModels.Category
{
    public class GetComicByTitle : Pagination
    {
        public string Title { get; set; }
    }
}
