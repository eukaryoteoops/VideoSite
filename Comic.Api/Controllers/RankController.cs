using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comic.Api.ReadModels.Rank;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSwag.Annotations;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class RankController : ControllerBase
    {
        private readonly IComicCounterRepository _comicCounterRepository;
        private readonly IComicFavoriteRepository _favoriteRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IMemoryCache _cache;

        public RankController(IComicCounterRepository comicCounterRepository, IComicFavoriteRepository favoriteRepository, IComicRepository comicRepository, IMemoryCache memoryCache, IHttpContextAccessor ctx)
        {
            _comicCounterRepository = comicCounterRepository;
            _favoriteRepository = favoriteRepository;
            _comicRepository = comicRepository;
            _cache = memoryCache;
        }

        /// <summary>
        ///     閱讀榜
        /// </summary>
        /// <returns></returns>
        [HttpGet("reading")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetReadingRank()
        {
            var result = await _cache.GetOrCreateAsync($"rank_reading", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                var startOfYesterday = now.Hour >= 18 ? now.Date.AddDays(-1).WithOffset(8).ToUnixTimeSeconds() : now.Date.AddDays(-2).WithOffset(8).ToUnixTimeSeconds();
                var endOfYesterday = now.Hour >= 18 ? now.Date.WithOffset(8).ToUnixTimeSeconds() : now.Date.AddDays(-1).WithOffset(8).ToUnixTimeSeconds();
                var counters = await _comicCounterRepository.GetAsync(o => o.CreatedTime >= startOfYesterday && o.CreatedTime <= endOfYesterday);
                var result = counters.GroupBy(o => o.ComicId).OrderByDescending(o => o.Count()).Select(o => new ComicBoxRM
                {
                    Id = o.Key,
                    Title = o.FirstOrDefault().Comic.Title,
                    ChapterCount = o.FirstOrDefault().Comic.ChapterCount,
                    UpdatedTime = o.FirstOrDefault().Comic.UpdatedTime
                });
                if (result.Count() < 10)
                {
                    var ids = result.Select(m => m.Id).ToList();
                    var comics = await _comicRepository.GetAsync(o => !ids.Contains(o.Id) && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds());
                    var randomComics = comics.OrderBy(o => new Random().Next()).Take(10 - result.Count()).Select(o => new ComicBoxRM
                    {
                        Id = o.Id,
                        Title = o.Title,
                        ChapterCount = o.ChapterCount,
                        UpdatedTime = o.UpdatedTime
                    });
                    result = result.Concat(randomComics).ToList();
                }
                return result;
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        /// <summary>
        ///     收藏榜
        /// </summary>
        /// <returns></returns>
        [HttpGet("favorite")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetFavoriteRank()
        {
            var result = await _cache.GetOrCreateAsync($"rank_favorite", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.DayOfWeek == DayOfWeek.Monday && now.Hour < 18 ?
                now.Date.AddHours(18).WithOffset(8) : now.DayOfWeek == DayOfWeek.Sunday ?
                now.Date.AddDays(1).AddHours(18).WithOffset(8) :
                now.Date.AddDays(8 - (int)now.DayOfWeek).AddHours(18).WithOffset(8);
                var startOfLastWeek = now.DayOfWeek == DayOfWeek.Monday && now.Hour < 18 ? now.Date.AddDays(-14) : now.DayOfWeek == DayOfWeek.Sunday ? now.Date.AddDays(-6).AddDays(-7) : now.Date.AddDays(-(int)now.DayOfWeek + 1).AddDays(-7);
                var endOfLastWeek = startOfLastWeek.AddDays(7);
                var startTime = startOfLastWeek.WithOffset(8).ToUnixTimeSeconds();
                var endTime = endOfLastWeek.WithOffset(8).ToUnixTimeSeconds();
                var favorites = await _favoriteRepository.GetAsync(o => o.CreatedTime >= startTime && o.CreatedTime < endTime);
                var result = favorites.GroupBy(o => o.ComicId).OrderByDescending(o => o.Count()).Select(o => new ComicBoxRM
                {
                    Id = o.Key,
                    Title = o.FirstOrDefault().Comic.Title,
                    ChapterCount = o.FirstOrDefault().Comic.ChapterCount,
                    UpdatedTime = o.FirstOrDefault().Comic.UpdatedTime
                });
                if (result.Count() < 10)
                {
                    var ids = result.Select(m => m.Id).ToList();
                    var comics = await _comicRepository.GetAsync(o => !ids.Contains(o.Id) && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds());
                    var randomComics = comics.OrderBy(o => new Random().Next()).Take(10 - result.Count()).Select(o => new ComicBoxRM
                    {
                        Id = o.Id,
                        Title = o.Title,
                        ChapterCount = o.ChapterCount,
                        UpdatedTime = o.UpdatedTime
                    });
                    result = result.Concat(randomComics).ToList();
                }
                return result;
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }
    }
}
