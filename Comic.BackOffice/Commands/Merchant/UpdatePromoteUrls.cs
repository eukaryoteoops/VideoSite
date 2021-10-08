using System.Collections.Generic;

namespace Comic.BackOffice.Commands.Merchant
{
    public class UpdatePromoteUrls
    {
        public int Id { get; set; }
        public List<string> Urls { get; set; }
    }
}
