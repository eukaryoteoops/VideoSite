using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IChapterPurchaseRepository : IBaseRepository<ChapterPurchases>
    {
        ValueTask PurchaseChapter(Members member, Chapters chapter);
    }
}
