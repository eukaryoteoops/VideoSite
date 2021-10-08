using System.Threading.Tasks;
using Comic.Domain.Entities;

namespace Comic.Domain.Repositories
{
    public interface IMemberRepository : IBaseRepository<Members>
    {
        ValueTask CompensateMember(Members member, byte type, int value, int managerId);
    }
}
