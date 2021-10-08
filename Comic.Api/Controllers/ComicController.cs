using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.Api.Commands.Comic;
using Comic.Api.QueryModels.Comic;
using Comic.Api.ReadModels.Comic;
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
    [ApiController]
    [Route("[controller]")]
    public class ComicController : ControllerBase
    {
        private readonly IComicCounterRepository _comicCounterRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IComicFavoriteRepository _comicfavoriteRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IComicTagMappingRepository _comicTagMappingRepository;
        private readonly IComicStatisticRepository _comicStatisticRepository;
        private readonly int _memberId;
        private readonly IMemoryCache _cache;

        public ComicController(IComicCounterRepository comicCounterRepository, IComicRepository comicRepository, IComicFavoriteRepository favoriteRepository, IChapterRepository chapterRepository, IComicTagMappingRepository comicTagMappingRepository, IHttpContextAccessor ctx, IMemoryCache memoryCache, IComicStatisticRepository comicStatisticRepository)
        {
            _comicCounterRepository = comicCounterRepository;
            _comicRepository = comicRepository;
            _comicfavoriteRepository = favoriteRepository;
            _chapterRepository = chapterRepository;
            _comicTagMappingRepository = comicTagMappingRepository;
            _cache = memoryCache;
            _memberId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "0");
            _comicStatisticRepository = comicStatisticRepository;
        }

        /// <summary>
        ///     紀錄點擊
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPatch("counter")]
        public async ValueTask<IActionResult> AddComicCounter(AddComicCounter cmd)
        {
            if (_memberId == 0) return Accepted();
            var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
            var startOfToday = now.Date.WithOffset(8).ToUnixTimeSeconds();
            var endOfToday = now.Date.AddDays(1).WithOffset(8).ToUnixTimeSeconds();
            var counter = await _comicCounterRepository.GetOneAsync(o => o.MemberId == _memberId && o.ComicId == cmd.ComicId && o.CreatedTime >= startOfToday && o.CreatedTime < endOfToday);
            if (counter != null) return Accepted();
            var newCounter = new ComicCounters(_memberId, cmd.ComicId);
            await _comicCounterRepository.AddAsync(newCounter);
            return Ok();
        }

        /// <summary>
        ///     限時免費
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("free")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetFree([FromQuery] GetFree qry)
        {
            var (comics, amount) = await _cache.GetOrCreateAsync($"free", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<Chapters, bool>> condition = o =>
                o.Comic.State == true && o.Comic.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                List<Expression<Func<Chapters, bool>>> lsExp = new List<Expression<Func<Chapters, bool>>>();
                lsExp.Add(o => o.Comic.Channel == ComicChannelEnum.同人誌);
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var chapters = await _chapterRepository.GetAsync(condition);
                var comics = chapters.OrderBy(o => o.Point).ThenByDescending(o => o.Comic.UpdatedTime).ThenByDescending(o => o.Comic.Id).Select(o => o.Comic).Distinct();
                return (comics, comics.Count());
            });
            comics = comics.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     新作發布
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("new_release")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetRelease([FromQuery] GetRelease qry)
        {
            var (comics, amount) = await _cache.GetOrCreateAsync($"new_release", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<Comics, bool>> condition = o =>
                o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
                lsExp.Add(o => o.Channel == ComicChannelEnum.韓漫);
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var comics = await _comicRepository.GetWithSortingAsync(condition, "UpdatedTime Desc, Id Desc");
                var amount = await _comicRepository.GetAmount(condition);
                return (comics, amount);
            });
            comics = comics.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     漫友最愛
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("everyone_like")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetFavorite([FromQuery] GetFavorite qry)
        {
            var (comics, amount) = await _cache.GetOrCreateAsync($"everyone_like", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                var startOfYesterday = now.Hour >= 18 ? now.Date.AddDays(-1).WithOffset(8).ToUnixTimeSeconds() : now.Date.AddDays(-2).WithOffset(8).ToUnixTimeSeconds();
                var endOfYesterday = now.Hour >= 18 ? now.Date.WithOffset(8).ToUnixTimeSeconds() : now.Date.AddDays(-1).WithOffset(8).ToUnixTimeSeconds();
                var favorites = await _comicfavoriteRepository.GetAsync(o => o.CreatedTime >= startOfYesterday && o.CreatedTime < endOfYesterday);
                var comicIds = favorites.GroupBy(o => o.ComicId).OrderBy(o => o.Count()).Select(o => o.Key).ToList();
                Expression<Func<Comics, bool>> condition = o =>
                    o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
                lsExp.Add(o => comicIds.Contains(o.Id));
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var comics = await _comicRepository.GetWithSortingAsync(condition);
                var amount = await _comicRepository.GetAmount(condition);
                comics = comicIds.Join(comics, o => o, o => o.Id, (key, item) => item);
                if (comics.Count() < 30)
                {
                    var ids = comics.Select(m => m.Id).ToList();
                    var rndComics = await _comicRepository.GetAsync(o => !ids.Contains(o.Id) && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds());
                    comics = comics.Concat(rndComics.OrderBy(o => new Random().Next()).Take(30 - comics.Count())).ToList();
                }
                return (comics, amount);
            });
            comics = comics.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     猜你喜歡
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("guess_like")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetGuess([FromQuery] GetGuess qry)
        {
            var (comics, amount) = await _cache.GetOrCreateAsync($"guess_like", async o =>
            {
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                Expression<Func<Comics, bool>> condition = o =>
                o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var comics = await _comicRepository.GetAsync(condition);
                var amount = await _comicRepository.GetAmount(condition);
                comics = comics.OrderBy(o => new Random().Next()).ToList();
                return (comics, amount);
            });
            comics = comics.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     完結大作
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("ended_comic")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetEnded([FromQuery] GetEnded qry)
        {
            var (comics, amount) = await _cache.GetOrCreateAsync($"ended_comic", async o =>
              {
                  var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
                  o.AbsoluteExpiration = now.Hour >= 18 ? now.AddDays(1).Date.AddHours(18).WithOffset(8) : now.Date.AddHours(18).WithOffset(8);
                  Expression<Func<Comics, bool>> condition = o =>
                     o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                  List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
                  lsExp.Add(o => o.IsEnded);
                  lsExp.Add(o => o.Channel == ComicChannelEnum.韓漫);
                  foreach (var exp in lsExp)
                      condition = condition.AndAlso(exp);
                  var comics = await _comicRepository.GetAsync(condition);
                  var amount = await _comicRepository.GetAmount(condition);
                  comics = comics.OrderBy(o => new Random().Next()).ToList();
                  return (comics, amount);
              });
            comics = comics.Skip((qry.PageNo - 1) * qry.PageSize).Take(qry.PageSize);
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     為你推薦
        /// </summary>
        /// <returns></returns>
        [HttpGet("recommend")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetRecommend(int count)
        {
            Expression<Func<Comics, bool>> condition = o =>
               o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
            List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
            lsExp.Add(o => o.UpdatedTime >= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).AddDays(-7).ToUnixTimeSeconds());
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var comics = await _comicRepository.GetAsync(condition);
            comics = comics.OrderBy(o => new Random().Next()).Take(count).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>()));
        }

        /// <summary>
        ///     漫畫頁一般資訊
        /// </summary>
        /// <param name="comicId"></param>
        /// <returns></returns>
        [HttpGet("-/{comicId}")]
        [SwaggerResponse(typeof(ComicInfoRM))]
        public async ValueTask<IActionResult> GetComicInfo([FromRoute] int comicId)
        {
            var result = await _cache.GetOrCreateAsync($"-/{comicId}", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                Expression<Func<Comics, bool>> condition = o =>
                    o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
                List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
                lsExp.Add(o => o.Id == comicId);
                foreach (var exp in lsExp)
                    condition = condition.AndAlso(exp);
                var comic = await _comicRepository.GetOneAsync(condition);
                if (comic == null) throw new Exception("not available");
                var chapters = await _chapterRepository.GetAsync(o => o.ComicId == comicId && o.EnabledTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds());
                var tags = await _comicTagMappingRepository.GetAsync(o => o.ComicId == comicId);
                var statistics = await _comicStatisticRepository.GetAsync(o => o.ComicId == comicId);
                return new ComicInfoRM(comic, chapters, tags.Select(o => o.Tag), statistics);
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }
    }
}
