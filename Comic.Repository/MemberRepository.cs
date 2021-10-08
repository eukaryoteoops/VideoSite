using System;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class MemberRepository : BaseRepository<Members>, IMemberRepository
    {
        private readonly IDbContext _db;
        ~MemberRepository()
        {
            _db.Dispose();
        }
        public MemberRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask CompensateMember(Members member, byte type, int value, int managerId)
        {
            _db.Session.BeginTransaction();
            try
            {
                var journal = new CompensationJournals(member.Id, type, value, managerId);
                await _db.InsertAsync(journal);
                if (type == 1)
                {
                    var pointJournal = new PointJournals(member.Id, value, "系統補單");
                    await _db.InsertAsync(pointJournal);
                    member.UpdatePoint(value);
                    await _db.UpdateAsync(member);
                }
                else if (type == 2)
                {
                    if (value == 9999) member.SetPremium();
                    else member.UpdatePackage(value);
                    await _db.UpdateAsync(member);
                }

            }
            catch (Exception ex)
            {
                _db.Session.RollbackTransaction();
                throw new Exception("CompensateMember Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
