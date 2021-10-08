using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.Api.Commands.Video;
using Comic.Api.QueryModels.Video;
using Comic.Api.ReadModels.Video;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSwag.Annotations;

namespace Comic.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IVideoTagMappingRepository _videoTagMappingRepository;
        private readonly IVideoActorMappingRepository _videoActorMappingRepository;
        private readonly IVideoCounterRepository _videoCounterRepository;
        private readonly IVideoStatisticRepository _videoStatisticRepository;
        private readonly IVideoChannelRepository _videoChannelRepository;
        private readonly IMemoryCache _cache;
        private readonly int _memberId;

        public VideoController(IVideoRepository videoRepository, IVideoTagMappingRepository videoTagMappingRepository, IMemoryCache cache, IHttpContextAccessor ctx, IVideoCounterRepository videoCounterRepository, IVideoStatisticRepository videoStatisticRepository, IVideoActorMappingRepository videoActorMappingRepository, IVideoChannelRepository videoChannelRepository)
        {
            _videoRepository = videoRepository;
            _videoTagMappingRepository = videoTagMappingRepository;
            _cache = cache;
            _memberId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "0");
            _videoCounterRepository = videoCounterRepository;
            _videoStatisticRepository = videoStatisticRepository;
            _videoActorMappingRepository = videoActorMappingRepository;
            _videoChannelRepository = videoChannelRepository;
        }

        /// <summary>
        ///     紀錄點擊 - 影片成功時
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPatch("counter")]
        public async ValueTask<IActionResult> AddVideoCounter(AddVideoCounter cmd)
        {
            if (_memberId == 0) return Accepted();
            var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var startOfToday = now.Date.WithOffset(8).ToUnixTimeSeconds();
            var endOfToday = now.Date.AddDays(1).WithOffset(8).ToUnixTimeSeconds();
            var counter = await _videoCounterRepository.GetOneAsync(o => o.MemberId == _memberId && o.Cid == cmd.Cid && o.CreatedTime >= startOfToday && o.CreatedTime < endOfToday);
            if (counter != null) return Accepted();
            var newCounter = new VideoCounters(_memberId, cmd.Cid);
            await _videoCounterRepository.AddAsync(newCounter);
            return Ok();
        }

        /// <summary>
        ///     最新上線
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("latest")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetLatest([FromQuery] GetLatest qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_latest", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<Videos, bool>> condition = o =>
                o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
                lsExp.Add(o => o.Channel != ChannelEnum.動畫 && o.Channel != ChannelEnum.動漫);
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoRepository.GetWithSortingAsync(condition, "EnabledDate Desc, Cid Desc");
                return (videos, videos.Count());
            });
            videos = videos.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     2021新番
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("2021")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> Get2021([FromQuery] Get2021 qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_2021", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<VideoTagMapping, bool>> condition = o =>
                o.Video.State == true && o.Video.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<VideoTagMapping, bool>>> lsExp = new List<Expression<Func<VideoTagMapping, bool>>>();
                lsExp.Add(o => o.Video.Channel == ChannelEnum.動漫);
                lsExp.Add(o => o.Tag.Name.Contains("2021"));
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoTagMappingRepository.GetAsync(condition);
                return (videos.Select(o => o.Video).Distinct(new VideoComparer()), videos.Count());
            });
            videos = videos.OrderByDescending(o => o.EnabledDate).Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     2020里番
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("2020")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> Get2020([FromQuery] Get2020 qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_2020", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<VideoTagMapping, bool>> condition = o =>
                o.Video.State == true && o.Video.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<VideoTagMapping, bool>>> lsExp = new List<Expression<Func<VideoTagMapping, bool>>>();
                lsExp.Add(o => o.Video.Channel == ChannelEnum.動漫);
                lsExp.Add(o => o.Tag.Name.Contains("2020"));
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoTagMappingRepository.GetAsync(condition);
                return (videos.Select(o => o.Video).Distinct(new VideoComparer()), videos.Count());
            });
            videos = videos.OrderByDescending(o => o.EnabledDate).Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }


        /// <summary>
        ///     NTR
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("ntr")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetNTR([FromQuery] GetNTR qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_ntr", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<VideoTagMapping, bool>> condition = o =>
                o.Video.State == true && o.Video.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<VideoTagMapping, bool>>> lsExp = new List<Expression<Func<VideoTagMapping, bool>>>();
                lsExp.Add(o => new string[] { "寝取り・寝取られ・NTR" }.Contains(o.Tag.Name));
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoTagMappingRepository.GetAsync(condition);
                return (videos.Select(o => o.Video).Distinct(new VideoComparer()), videos.Count());
            });
            videos = videos.OrderByDescending(o => o.EnabledDate).Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     制服AV
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("uniform")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetUniform([FromQuery] GetUniform qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_uniform", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<VideoTagMapping, bool>> condition = o =>
                o.Video.State == true && o.Video.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<VideoTagMapping, bool>>> lsExp = new List<Expression<Func<VideoTagMapping, bool>>>();
                lsExp.Add(o => new string[] { "制服", "女子大生", "メイド" }.Contains(o.Tag.Name));
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoTagMappingRepository.GetAsync(condition);
                return (videos.Select(o => o.Video).Distinct(new VideoComparer()), videos.Count());
            });
            videos = videos.OrderByDescending(o => o.EnabledDate).Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     元气美少女
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("beauty")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetBeauty([FromQuery] GetBeauty qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_beauty", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<VideoTagMapping, bool>> condition = o =>
                o.Video.State == true && o.Video.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<VideoTagMapping, bool>>> lsExp = new List<Expression<Func<VideoTagMapping, bool>>>();
                lsExp.Add(o => new string[] { "美少女" }.Contains(o.Tag.Name));
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoTagMappingRepository.GetAsync(condition);
                return (videos.Select(o => o.Video).Distinct(new VideoComparer()), videos.Count());
            });
            videos = videos.OrderByDescending(o => o.EnabledDate).Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }
        /// <summary>
        ///     網黃主播
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("streamer")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetStreamer([FromQuery] GetStreamer qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_streamer", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<Videos, bool>> condition = o =>
                o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
                lsExp.Add(o => o.Channel == ChannelEnum.免費 || o.Channel == ChannelEnum.自拍);
                lsExp.Add(o => o.Name.Contains("主播"));
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoRepository.GetWithSortingAsync(condition, "EnabledDate Desc, Cid Desc");
                return (videos, videos.Count());
            });
            videos = videos.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     取得可用頻道
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("channels")]
        [SwaggerResponse(typeof(IEnumerable<VideoChannelRM>))]
        public async ValueTask<IActionResult> GetVChannels()
        {
            var channels = await _cache.GetOrCreateAsync($"video_channels", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);
                var channels = await _videoChannelRepository.GetWithSortingAsync(o => o.State, "Order Asc");
                return channels;
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(channels.Adapt<IEnumerable<VideoChannelRM>>()));
        }

        /// <summary>
        ///     頻道頁籤影片
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("channel/{Channel}")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetVideosByChannel([FromQuery] GetByChannel qry)
        {
            var (videos, amount) = await _cache.GetOrCreateAsync($"video_{(ChannelEnum)qry.Channel}", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<Videos, bool>> condition = o =>
                o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
                lsExp.Add(o => o.Channel == (ChannelEnum)qry.Channel);
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var videos = await _videoRepository.GetWithSortingAsync(condition, "EnabledDate Desc, Cid Desc");
                return (videos, videos.Count());
            });
            videos = videos.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     為你推薦
        /// </summary>
        /// <returns></returns>
        [HttpGet("recommend")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetRecommend(int count)
        {
            Expression<Func<Videos, bool>> condition = o =>
               o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
            List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
            lsExp.Add(o => o.EnabledDate >= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).AddDays(-7).ToDateInteger());
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var comics = await _videoRepository.GetAsync(condition);
            comics = comics.OrderBy(o => new Random().Next()).Take(count).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<VideoBoxRM>>()));
        }

        /// <summary>
        ///     可能喜歡
        /// </summary>
        /// <returns></returns>
        [HttpGet("probably_like")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetProbablyLike(string cid, int channel, int count)
        {
            var videos = await _cache.GetOrCreateAsync($"probably_like_{channel}", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                Expression<Func<Videos, bool>> condition = o =>
                  o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
                lsExp.Add(o => o.Channel == (ChannelEnum)channel);
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                return await _videoRepository.GetAsync(condition);
            });

            videos = videos.Where(o => o.Cid != cid).OrderBy(o => new Random().Next()).Take(count).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>()));
        }

        /// <summary>
        ///     視頻頁一般資訊
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [HttpGet("-/{cid}")]
        [SwaggerResponse(typeof(VideoInfoRM))]
        public async ValueTask<IActionResult> GetVideoInfo([FromRoute] string cid)
        {
            var result = await _cache.GetOrCreateAsync($"-/{cid}", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                Expression<Func<Videos, bool>> condition = o =>
                    o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
                List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
                lsExp.Add(o => o.Cid == cid);
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var video = await _videoRepository.GetOneAsync(condition);
                if (video == null) throw new Exception("not available");
                var tags = await _videoTagMappingRepository.GetAsync(o => o.VideoId == cid);
                var actors = await _videoActorMappingRepository.GetAsync(o => o.VideoId == cid);
                var statistics = await _videoStatisticRepository.GetAsync(o => o.Cid == cid);
                return new VideoInfoRM(video, tags.Select(o => o.Tag), actors.Select(o => o.Actor), statistics);
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }
    }

    internal class VideoComparer : IEqualityComparer<Videos>
    {
        public bool Equals([AllowNull] Videos x, [AllowNull] Videos y)
        {
            return x.Cid == y.Cid;
        }

        public int GetHashCode([DisallowNull] Videos obj)
        {
            return obj.Cid.GetHashCode();
        }
    }
}
