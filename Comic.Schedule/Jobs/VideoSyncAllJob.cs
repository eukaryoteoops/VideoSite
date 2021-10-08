using System;
using System.Collections.Generic;
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
    public class VideoSyncJob
    {
        private readonly IVideoRepository _videoRepository;
        public VideoSyncJob(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async ValueTask Trigger(PerformContext ctx)
        {
            var sid = "TT33JXNEB2";//SF9EXG9DG2 //TT33JXNEB2
            var size = 100;//int.MaxValue
            var date = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyyMMdd");
            var uri = new Uri($"https://api.vp.vscp168.com/api/video/all/release?sid={sid}&date={date}");
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            var resp = await new HttpClient().SendAsync(req);
            var source = JsonSerializer.Deserialize<SourceVideoResponse>(await resp.Content.ReadAsStringAsync());
            var entities = source.data.Select(o => new Videos(o.cid, o.ch, o.name, o.desc, o.v_url, o.p_url, o.enable_date, o.tag, o.actor)).ToList();
            var bar = ctx.WriteProgressBar();
            foreach (var item in entities.WithProgress(bar))
            {
                try
                {
                    await _videoRepository.InsertVideo(item);
                    ctx.WriteLine($"{item.Cid} done.");
                }
                catch (Exception ex)
                {
                    ctx.WriteLine($"{item.Cid} error {ex.Message}");
                    continue;
                }
            }
        }
    }

    public class SourceVideoResponse
    {
        public int code { get; set; }
        public string desc { get; set; }
        public IEnumerable<SourceVideo> data { get; set; }
    }
    public class SourceVideo
    {
        public string cid { get; set; }
        public int state { get; set; }
        public string ch { get; set; }

        public int is_1200 { get; set; }
        public int is_600 { get; set; }
        public int is_300 { get; set; }

        public string v_url { get; set; }
        public string p_url { get; set; }
        public string name { get; set; }

        public string actor { get; set; }
        public string tag { get; set; }
        public string vendor { get; set; }

        public string desc { get; set; }
        public long utime { get; set; }
        public int clicks { get; set; }

        public int enable_date { get; set; }
    }
}
