
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comic.BackOffice.ReadModels.Home;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSwag.Annotations;

namespace Comic.BackOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IVideoCounterRepository _videoCounterRepository;
        private readonly IComicCounterRepository _comicCounterRepository;
        private readonly IMemoryCache _cache;

        public HomeController(IOrderRepository orderRepository, IMemoryCache cache, IVideoCounterRepository videoCounterRepository, IComicCounterRepository comicCounterRepository)
        {
            _orderRepository = orderRepository;
            _cache = cache;
            _videoCounterRepository = videoCounterRepository;
            _comicCounterRepository = comicCounterRepository;
        }

        [HttpGet("purchase_statistic")]
        [SwaggerResponse(typeof(List<PurchaseStatisticRM>))]
        public async Task<IActionResult> GetPurchaseStatistic()
        {
            var result = await _cache.GetOrCreateAsync($"purchase_statistic", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                var start = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).Date.AddDays(-8).WithOffset(8).ToUnixTimeSeconds();
                var orders = await _orderRepository.GetAsync(o => o.State && o.CreatedTime >= start && o.CreatedTime <= now);
                var repurchaseIds = await _orderRepository.GetRePurchaseMembers();
                return orders.GroupBy(o => DateTimeOffset.FromUnixTimeSeconds(o.CreatedTime).ToOffset(TimeSpan.FromHours(8)).Date.ToShortDateString()).OrderByDescending(o => o.Key).Select(o => new PurchaseStatisticRM(o.Key, o.ToList(), repurchaseIds));
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        [HttpGet("video_counter_statistic")]
        [SwaggerResponse(typeof(List<VideoCounterStatisticRM>))]
        public async Task<IActionResult> GetVideoCounterStatistic()
        {
            var result = await _cache.GetOrCreateAsync($"video_counter_statistic", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                var start = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).Date.AddDays(-8).WithOffset(8).ToUnixTimeSeconds();
                var videoCounters = await _videoCounterRepository.GetAsync(o => o.CreatedTime >= start && o.CreatedTime <= now);
                return videoCounters.GroupBy(o => DateTimeOffset.FromUnixTimeSeconds(o.CreatedTime).ToOffset(TimeSpan.FromHours(8)).Date.ToShortDateString()).OrderByDescending(o => o.Key).Select(o => new VideoCounterStatisticRM(o.Key, o.ToList()));
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        [HttpGet("comic_counter_statistic")]
        [SwaggerResponse(typeof(List<ComicCounterStatisticRM>))]
        public async Task<IActionResult> GetComicCounterStatistic()
        {
            var result = await _cache.GetOrCreateAsync($"comic_counter_statistic", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                var start = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).Date.AddDays(-8).WithOffset(8).ToUnixTimeSeconds();
                var comicCounters = await _comicCounterRepository.GetAsync(o => o.CreatedTime >= start && o.CreatedTime <= now);
                return comicCounters.GroupBy(o => DateTimeOffset.FromUnixTimeSeconds(o.CreatedTime).ToOffset(TimeSpan.FromHours(8)).Date.ToShortDateString()).OrderByDescending(o => o.Key).Select(o => new ComicCounterStatisticRM(o.Key, o.ToList()));
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }
    }
}
