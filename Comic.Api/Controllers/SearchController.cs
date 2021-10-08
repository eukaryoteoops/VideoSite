using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.Api.QueryModels.Search;
using Comic.Api.ReadModels.Search;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSwag.Annotations;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class SearchController : ControllerBase
    {
        private readonly IComicRepository _comicRepository;
        private readonly IComicTagMappingRepository _comicTagMappingRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IVideoTagMappingRepository _videoTagMappingRepository;
        private readonly IVideoActorMappingRepository _videoActorMappingRepository;
        private readonly IMemoryCache _cache;

        public SearchController(IComicRepository comicRepository, IComicTagMappingRepository comicTagMappingRepository, IVideoTagMappingRepository videoTagMappingRepository, IVideoRepository videoRepository, IVideoActorMappingRepository videoActorMappingRepository, IMemoryCache cache)
        {
            _comicRepository = comicRepository;
            _comicTagMappingRepository = comicTagMappingRepository;
            _videoTagMappingRepository = videoTagMappingRepository;
            _videoRepository = videoRepository;
            _videoActorMappingRepository = videoActorMappingRepository;
            _cache = cache;
        }

        /// <summary>
        ///     搜尋頁取50個標籤 - 漫畫
        /// </summary>
        /// <returns></returns>
        [HttpGet("comic/rnd_tag")]
        [SwaggerResponse(typeof(IEnumerable<TagRM>))]
        public async ValueTask<IActionResult> GetComicTags()
        {
            var tags = await _cache.GetOrCreateAsync($"comic_rnd_tag", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                var tags = await _comicTagMappingRepository.GetAsync(o => o.Comic.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds());
                return tags.Select(o => o.Tag).GroupBy(o => o.Name).Select(o => o.First());
            });
            var result = tags.OrderBy(o => new Random().Next()).Take(50);
            return Ok(ResponseUtility.CreateSuccessResopnse(result.Adapt<IEnumerable<TagRM>>()));
        }

        /// <summary>
        ///     搜尋頁取50個標籤 - 視頻
        /// </summary>
        /// <returns></returns>
        [HttpGet("video/rnd_tag")]
        [SwaggerResponse(typeof(IEnumerable<TagRM>))]
        public async ValueTask<IActionResult> GetVideoTags()
        {
            var tags = await _cache.GetOrCreateAsync($"video_rnd_tag", async o =>
            {
                o.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                var tags = await _videoTagMappingRepository.GetAsync(o => o.Video.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger());
                return tags.Select(o => o.Tag).GroupBy(o => o.Name).Select(o => o.First());
            });
            var result = tags.OrderBy(o => new Random().Next()).Take(50);
            return Ok(ResponseUtility.CreateSuccessResopnse(result.Adapt<IEnumerable<TagRM>>()));
        }

        /// <summary>
        ///     搜尋標題 - 漫畫
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("comic/title")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetComicByTitle([FromQuery] GetComicByTitle qry)
        {
            Expression<Func<Comics, bool>> condition = o => o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
            List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
            lsExp.Add(o => o.Title.Contains(qry.Title.Trim()));
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var comics = await _comicRepository.GetWithSortingAsync(condition, "UpdatedTime Desc, Id Desc", qry.PageNo, qry.PageSize);
            var amount = await _comicRepository.GetAmount(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     搜尋標籤 - 漫畫
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("comic/tag")]
        [SwaggerResponse(typeof(IEnumerable<ComicBoxRM>))]
        public async ValueTask<IActionResult> GetComicByTag([FromQuery] GetComicByTag qry)
        {
            Expression<Func<Comics, bool>> condition = o => o.State == true && o.UpdatedTime <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
            List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
            var mapping = await _comicTagMappingRepository.GetAsync(o => o.Tag.Name == qry.TagName);
            var comicIds = mapping.Select(o => o.ComicId).Distinct();
            lsExp.Add(o => comicIds.Contains(o.Id));
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var comics = await _comicRepository.GetWithSortingAsync(condition, "UpdatedTime Desc, Id Desc", qry.PageNo, qry.PageSize);
            var amount = await _comicRepository.GetAmount(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     搜尋標籤 - 視頻
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("video/tag")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetVideoByTag([FromQuery] GetVideoByTag qry)
        {
            Expression<Func<Videos, bool>> condition = o => o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
            List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
            var mapping = await _videoTagMappingRepository.GetAsync(o => o.Tag.Name == qry.TagName);
            var comicIds = mapping.Select(o => o.VideoId).Distinct();
            lsExp.Add(o => comicIds.Contains(o.Cid));
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var videos = await _videoRepository.GetWithSortingAsync(condition, "EnabledDate Desc, Cid Desc", qry.PageNo, qry.PageSize);
            var amount = await _videoRepository.GetAmount(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     搜尋標題 - 視頻
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("video/title")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetVideoByTitle([FromQuery] GetVideoByTitle qry)
        {
            Expression<Func<Videos, bool>> condition = o => o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
            List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
            lsExp.Add(o => o.Name.Contains(qry.Title.Trim()));
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var videos = await _videoRepository.GetWithSortingAsync(condition, "EnabledDate Desc, Cid Desc", qry.PageNo, qry.PageSize);
            var amount = await _videoRepository.GetAmount(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        /// <summary>
        ///     搜尋演員 - 視頻
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet("video/actor")]
        [SwaggerResponse(typeof(IEnumerable<VideoBoxRM>))]
        public async ValueTask<IActionResult> GetVideoByActor([FromQuery] GetVideoByActor qry)
        {
            Expression<Func<Videos, bool>> condition = o => o.State == true && o.EnabledDate <= DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToDateInteger();
            List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
            var mapping = await _videoActorMappingRepository.GetAsync(o => o.Actor.Name == qry.Name);
            var comicIds = mapping.Select(o => o.VideoId).Distinct();
            lsExp.Add(o => comicIds.Contains(o.Cid));
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var videos = await _videoRepository.GetWithSortingAsync(condition, "EnabledDate Desc, Cid Desc", qry.PageNo, qry.PageSize);
            var amount = await _videoRepository.GetAmount(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(videos.Adapt<IEnumerable<VideoBoxRM>>(), qry.PageNo, qry.PageSize, amount));
        }
    }
}
