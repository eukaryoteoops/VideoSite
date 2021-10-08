namespace Comic.BackOffice.Merchant.ReadModels.Home
{
    public class AnnouncementRM
    {
        public AnnouncementRM(string merchantAnnouncement)
        {
            MerchantAnnouncement = merchantAnnouncement;
        }

        public string MerchantAnnouncement { get; }
    }
}
