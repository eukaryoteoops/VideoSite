namespace Comic.BackOffice.ReadModels.Video
{
    public class VideoRM
    {
        public string Cid { get; set; }
        public string Name { get; set; }
        public byte Channel { get; set; }
        public int EnabledDate { get; set; }
        /// <summary>
        ///     0 : 免費
        ///     0以外 : VIP
        /// </summary>
        public int Point { get; set; }
        public bool State { get; set; }


    }
}
