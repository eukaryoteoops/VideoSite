namespace Comic.BackOffice.QueryModels.Advert
{
    public class GetAdverts
    {
        /// <summary>
        ///     1 : 漫畫廣告
        ///     2 : 會員廣告
        ///     3 : 輪播廣告
        ///     4 : Web蓋板廣告
        ///     5 : App蓋板廣告
        ///     6 : 燈箱廣告
        /// </summary>
        public byte Type { get; set; }
    }
}
