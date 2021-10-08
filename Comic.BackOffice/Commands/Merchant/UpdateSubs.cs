using System.Collections.Generic;

namespace Comic.BackOffice.Commands.Merchant
{
    public class UpdateSubs
    {
        public int ParentId { get; set; }
        public List<int> ChildrenIds { get; set; }
    }
}
