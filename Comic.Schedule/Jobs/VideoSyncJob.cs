using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Hangfire.Console;
using Hangfire.Server;

namespace Comic.Schedule.Jobs
{
    public class VideoSyncAllJob
    {
        private readonly IVideoRepository _videoRepository;
        public VideoSyncAllJob(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async ValueTask Trigger(PerformContext ctx)
        {
            var sid = "TT33JXNEB2";//SF9EXG9DG2 //TT33JXNEB2
            var size = 100;//int.MaxValue
            var date = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyyMMdd");
            for (int i = 1; i < 377; i++)
            {
                var uri = new Uri($"https://api.vp.vscp168.com/api/video/all?sid={sid}&pageNo={i}&pageSize={size}");
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                var resp = await new HttpClient().SendAsync(req);
                var source = JsonSerializer.Deserialize<SourceVideoResponse>(await resp.Content.ReadAsStringAsync());
                var entities = source.data.Select(o => new Videos(o.cid, o.ch, o.name, o.desc, o.v_url, o.p_url, o.enable_date, o.tag, o.actor)).ToList();
                foreach (var item in entities)
                {
                    try
                    {
                        await _videoRepository.InsertVideo(item);
                    }
                    catch (Exception ex)
                    {
                        ctx.WriteLine($"{item.Cid} error {ex.Message}");
                        continue;
                    }
                }
                ctx.WriteLine($"page {i} done.");
            }
        }
    }
}
