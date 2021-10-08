using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class MerchantSubs : Entity
    {
        public MerchantSubs()
        {
        }

        public MerchantSubs(int parentId, int childId)
        {
            ParentId = parentId;
            ChildId = childId;
        }

        public int ParentId { get; set; }
        public int ChildId { get; set; }
        [Navigation("ChildId")]
        public Merchants Merchants { get; set; }
    }
}
