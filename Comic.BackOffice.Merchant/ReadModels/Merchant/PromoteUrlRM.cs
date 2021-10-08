using System.Collections.Generic;
using System.Linq;
using Comic.Domain.Entities;

namespace Comic.BackOffice.Merchant.ReadModels.Merchant
{
    public class PromoteUrlRM
    {
        public PromoteUrlRM(Configs config, IEnumerable<PromoteUrls> urls, int Id)
        {
            PermanentUrl = config.PermanentUrl.Insert(config.PermanentUrl.IndexOf("//") + 2, $"{Id}.");
            LatestUrl = config.LatestUrl.Insert(config.LatestUrl.IndexOf("//") + 2, $"{Id}.");
            SiteUrl = config.SiteUrl.Insert(config.SiteUrl.IndexOf("//") + 2, $"{Id}.");
            Urls = urls.Select(o => o.Url);
        }

        public string PermanentUrl { get; set; }
        public string LatestUrl { get; set; }
        public string SiteUrl { get; set; }
        public IEnumerable<string> Urls { get; set; }
    }
}
