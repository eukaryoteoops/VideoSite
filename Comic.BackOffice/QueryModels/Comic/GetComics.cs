namespace Comic.BackOffice.QueryModels.Comic
{
    public class GetComics : Pagination
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public bool? IsEnded { get; set; }
        public bool? State { get; set; }
        public byte? Channel { get; set; }
    }
}
