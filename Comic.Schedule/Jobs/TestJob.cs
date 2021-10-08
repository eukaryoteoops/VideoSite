using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Hangfire.Server;

namespace Comic.Schedule.Jobs
{
    public class TestJob
    {
        private readonly IDbContext dbContext;
        public TestJob(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async ValueTask Trigger(PerformContext ctx)
        {
            var members = dbContext.Query<Members>().ToList();
            var result = members.GroupBy(o => DateTimeOffset.FromUnixTimeSeconds(o.CreatedTime).ToOffset(TimeSpan.FromHours(8)).Date).OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.GroupBy(m => m.MerchantId).ToDictionary(m => m.Key, m => m.Count()));
            var a = JsonSerializer.Serialize(result);
        }
    }
}
