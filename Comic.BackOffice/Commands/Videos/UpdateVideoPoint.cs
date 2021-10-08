namespace Comic.BackOffice.Commands.Videos
{
    public class UpdateVideoPoint
    {
        public string Cid { get; set; }

        /// <summary>
        ///     0 : 免費
        ///     0以外 : VIP
        /// </summary>
        public int Point { get; set; }
    }
}
