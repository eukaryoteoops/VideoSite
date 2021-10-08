using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comic.Api.Commands.Shelf;
using Comic.Api.ReadModels.Shelf;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShelfController : ControllerBase
    {
        private readonly IComicHistoryRepository _comicHistoryRepository;
        private readonly IComicFavoriteRepository _comicFavoriteRepository;
        private readonly IVideoHistoryRepository _videoHistoryRepository;
        private readonly IVideoFavoriteRepository _videoFavoriteRepository;
        private readonly int _memberId;

        public ShelfController(IComicHistoryRepository readingHistoryRepository, IComicFavoriteRepository favoriteRepository, IHttpContextAccessor ctx, IVideoFavoriteRepository videoFavoriteRepository, IVideoHistoryRepository videoHistoryRepository)
        {
            _comicHistoryRepository = readingHistoryRepository;
            _comicFavoriteRepository = favoriteRepository;
            _videoHistoryRepository = videoHistoryRepository;
            _videoFavoriteRepository = videoFavoriteRepository;
            _memberId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "0");
        }

        /// <summary>
        ///     瀏覽紀錄 - 漫畫
        /// </summary>
        /// <returns></returns>
        [HttpGet("history/comic")]
        [SwaggerResponse(typeof(IEnumerable<ComicHistoryRM>))]
        public async ValueTask<IActionResult> GetComicHistory()
        {
            var histories = await _comicHistoryRepository.GetAsync(o => o.MemberId == _memberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(histories.OrderByDescending(o => o.ReadingTime).Adapt<IEnumerable<ComicHistoryRM>>()));
        }

        /// <summary>
        ///     瀏覽紀錄 - 視頻
        /// </summary>
        /// <returns></returns>
        [HttpGet("history/video")]
        [SwaggerResponse(typeof(IEnumerable<VideoHistoryRM>))]
        public async ValueTask<IActionResult> GetVideoHistory()
        {
            var histories = await _videoHistoryRepository.GetAsync(o => o.MemberId == _memberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(histories.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<VideoHistoryRM>>()));
        }

        /// <summary>
        ///     收藏列表 - 漫畫
        /// </summary>
        /// <returns></returns>
        [HttpGet("favorite/comic")]
        [SwaggerResponse(typeof(IEnumerable<ComicFavoriteRM>))]
        public async ValueTask<IActionResult> GetComicFavorite()
        {

            var favorites = await _comicFavoriteRepository.GetAsync(o => o.MemberId == _memberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(favorites.OrderByDescending(o => o.Comic.UpdatedTime).Adapt<IEnumerable<ComicFavoriteRM>>()));
        }

        /// <summary>
        ///     收藏列表 - 視頻
        /// </summary>
        /// <returns></returns>
        [HttpGet("favorite/video")]
        [SwaggerResponse(typeof(IEnumerable<VideoFavoriteRM>))]
        public async ValueTask<IActionResult> GetVideoFavorite()
        {
            var favorites = await _videoFavoriteRepository.GetAsync(o => o.MemberId == _memberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(favorites.OrderByDescending(o => o.Video.EnabledDate).Adapt<IEnumerable<VideoFavoriteRM>>()));
        }

        /// <summary>
        ///     加入瀏覽紀錄 - 漫畫
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPatch("history/comic")]
        public async ValueTask<IActionResult> AddComicHistory(AddComicHistory cmd)
        {
            var history = await _comicHistoryRepository.GetOneAsync(o => o.MemberId == _memberId && o.ComicId == cmd.ComicId);
            if (history == null)
            {
                var newHistory = new ComicHistories(_memberId, cmd.ComicId, cmd.Chapter);
                await _comicHistoryRepository.AddAsync(newHistory);
            }
            else
            {
                history.UpdateHistory(cmd.Chapter);
                await _comicHistoryRepository.UpdateAsync(history);
            }
            return Ok();
        }

        /// <summary>
        ///     加入瀏覽紀錄 - 視頻
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPatch("history/video")]
        public async ValueTask<IActionResult> AddVideoHistory(AddVideoHistory cmd)
        {
            var history = await _videoHistoryRepository.GetOneAsync(o => o.MemberId == _memberId && o.Cid == cmd.Cid);
            if (history == null)
            {
                var newHistory = new VideoHistories(_memberId, cmd.Cid);
                await _videoHistoryRepository.AddAsync(newHistory);
            }
            else
            {
                history.UpdateHistory();
                await _videoHistoryRepository.UpdateAsync(history);
            }
            return Ok();
        }

        /// <summary>
        ///     加入收藏 - 漫畫
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPatch("favorite/comic")]
        public async ValueTask<IActionResult> AddFavorite(AddComicFavorite cmd)
        {
            var favorite = await _comicFavoriteRepository.GetOneAsync(o => o.MemberId == _memberId && o.ComicId == cmd.ComicId);
            if (favorite == null)
            {
                var newFavorite = new ComicFavorites(_memberId, cmd.ComicId);
                await _comicFavoriteRepository.AddAsync(newFavorite);
            }
            return Ok();
        }

        /// <summary>
        ///     加入收藏 - 視頻
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPatch("favorite/video")]
        public async ValueTask<IActionResult> AddVideoFavorite(AddVideoFavorite cmd)
        {
            var favorite = await _videoFavoriteRepository.GetOneAsync(o => o.MemberId == _memberId && o.Cid == cmd.Cid);
            if (favorite == null)
            {
                var newFavorite = new VideoFavorites(_memberId, cmd.Cid);
                await _videoFavoriteRepository.AddAsync(newFavorite);
            }
            return Ok();
        }

        /// <summary>
        ///     移除瀏覽紀錄 - 漫畫
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpDelete("history/comic")]
        public async ValueTask<IActionResult> DeleteComicHistory(DeleteComicHistory cmd)
        {
            var history = await _comicHistoryRepository.GetOneAsync(o => o.MemberId == _memberId && o.Id == cmd.Id);
            await _comicHistoryRepository.DeleteAsync(history);
            return Ok();
        }

        /// <summary>
        ///     移除瀏覽紀錄 - 視頻
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpDelete("history/video")]
        public async ValueTask<IActionResult> DeleteVideoHistory(DeleteVideoHistory cmd)
        {
            var history = await _videoHistoryRepository.GetOneAsync(o => o.MemberId == _memberId && o.Id == cmd.Id);
            await _videoHistoryRepository.DeleteAsync(history);
            return Ok();
        }

        /// <summary>
        ///     移除收藏 - 漫畫
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpDelete("favorite/comic")]
        public async ValueTask<IActionResult> DeleteComicFavorite(DeleteComicFavorite cmd)
        {
            var favorite = await _comicFavoriteRepository.GetOneAsync(o => o.MemberId == _memberId && o.ComicId == cmd.ComicId);
            await _comicFavoriteRepository.DeleteAsync(favorite);
            return Ok();
        }

        /// <summary>
        ///     移除收藏 - 視頻
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpDelete("favorite/video")]
        public async ValueTask<IActionResult> DeleteVideoFavorite(DeleteVideoFavorite cmd)
        {
            var favorite = await _videoFavoriteRepository.GetOneAsync(o => o.MemberId == _memberId && o.Cid == cmd.Cid);
            await _videoFavoriteRepository.DeleteAsync(favorite);
            return Ok();
        }


    }
}
