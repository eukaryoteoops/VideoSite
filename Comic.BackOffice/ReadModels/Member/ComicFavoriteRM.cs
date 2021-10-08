namespace Comic.BackOffice.ReadModels.Member
{
    public class ComicFavoriteRM
    {
        public int Id { get; set; }
        public int ComicId { get; set; }
        public string ComicTitle { get; set; }
        public int Chapter { get; set; }
        public long UpdatedTime { get; set; }
    }
}
