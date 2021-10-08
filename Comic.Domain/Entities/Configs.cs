namespace Comic.Domain.Entities
{
    public class Configs : Entity
    {
        public string MerchantAnnouncement { get; set; }
        public string MemberAnnouncement { get; set; }
        public string IosVersion { get; set; }
        public string IosUrl { get; set; }
        public string IosBackupUrl { get; set; }
        public string AndroidVersion { get; set; }
        public string AndroidUrl { get; set; }
        public string PermanentUrl { get; set; }
        public string LatestUrl { get; set; }
        public string SiteUrl { get; set; }
        public string ImageUrl { get; set; }
        public string ReleaseUrl { get; set; }
        public string VideoDomain { get; set; }


        public void UpdateMerchantAnnouncement(string content)
        {
            this.MerchantAnnouncement = string.IsNullOrEmpty(content) ? this.MerchantAnnouncement : content;
        }

        public void UpdateMemberAnnouncement(string content)
        {
            this.MemberAnnouncement = string.IsNullOrEmpty(content) ? this.MemberAnnouncement : content;

        }

        public void UpdateConfigs(string iosVersion, string iosUrl, string iosBackupUrl, string androidVersion, string androidUrl, string permanentUrl, string latestUrl, string siteUrl, string imageUrl, string releaseUrl, string videoDomain)
        {
            this.IosVersion = string.IsNullOrEmpty(iosVersion) ? this.IosVersion : iosVersion;
            this.IosUrl = string.IsNullOrEmpty(iosUrl) ? this.IosUrl : iosUrl;
            this.IosBackupUrl = iosBackupUrl;
            this.AndroidVersion = string.IsNullOrEmpty(androidVersion) ? this.AndroidVersion : androidVersion;
            this.AndroidUrl = string.IsNullOrEmpty(androidUrl) ? this.AndroidUrl : androidUrl;
            this.PermanentUrl = permanentUrl;
            this.LatestUrl = latestUrl;
            this.SiteUrl = string.IsNullOrEmpty(siteUrl) ? this.SiteUrl : siteUrl;
            this.ImageUrl = string.IsNullOrEmpty(imageUrl) ? this.ImageUrl : imageUrl;
            this.ReleaseUrl = string.IsNullOrEmpty(releaseUrl) ? this.ReleaseUrl : releaseUrl;
            this.VideoDomain = string.IsNullOrEmpty(videoDomain) ? this.VideoDomain : videoDomain;
        }
    }
}
