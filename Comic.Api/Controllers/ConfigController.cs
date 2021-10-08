using System;
using System.Threading.Tasks;
using Comic.Api.ReadModels.Config;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class ConfigController : ControllerBase
    {
        private readonly IPromoteUrlRepository _promoteUrlRepository;
        private readonly ICache<Configs> _configCache;

        public ConfigController(IPromoteUrlRepository promoteUrlRepository, ICache<Configs> configCache)
        {
            _promoteUrlRepository = promoteUrlRepository;
            _configCache = configCache;
        }

        /// <summary>
        ///     會員頁設定資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(typeof(ConfigRM))]
        public async ValueTask<IActionResult> Get()
        {
            var configs = await _configCache.GetOneAsync($"{CacheKeys.Configs}", null, TimeSpan.FromHours(1));
            return Ok(ResponseUtility.CreateSuccessResopnse(configs.Adapt<ConfigRM>()));
        }

        /// <summary>
        ///     圖片網域
        /// </summary>
        /// <returns></returns>
        [HttpGet("pic_url")]
        [SwaggerResponse(typeof(PicUrlRM))]
        public async ValueTask<IActionResult> GetPicUrl()
        {
            var configs = await _configCache.GetOneAsync($"{CacheKeys.Configs}", null, TimeSpan.FromHours(1));
            return Ok(ResponseUtility.CreateSuccessResopnse(configs.Adapt<PicUrlRM>()));
        }

        /// <summary>
        ///     影片網域
        /// </summary>
        /// <returns></returns>
        [HttpGet("video_url")]
        [SwaggerResponse(typeof(VideoUrlRM))]
        public async ValueTask<IActionResult> GetVideoUrl()
        {
            var configs = await _configCache.GetOneAsync($"{CacheKeys.Configs}", null, TimeSpan.FromHours(1));
            return Ok(ResponseUtility.CreateSuccessResopnse(configs.Adapt<VideoUrlRM>()));
        }

        /// <summary>
        ///     發布頁設定資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet("release")]
        [SwaggerResponse(typeof(ReleasePageRM))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> GetReleasePage()
        {
            var configs = await _configCache.GetOneAsync($"{CacheKeys.Configs}", null, TimeSpan.FromHours(1));
            return Ok(ResponseUtility.CreateSuccessResopnse(configs.Adapt<ReleasePageRM>()));
        }

        /// <summary>
        ///     domain歸屬
        /// </summary>
        /// <returns></returns>
        [HttpGet("merchant")]
        [SwaggerResponse(typeof(MerchantIdRM))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> GetMerchantId(string domain)
        {
            var uri = new Uri(domain);
            var configs = await _configCache.GetOneAsync($"{CacheKeys.Configs}", null, TimeSpan.FromHours(1));
            if (domain.Contains(new Uri(configs.SiteUrl).Host) || domain.Contains(new Uri(configs.LatestUrl).Host) || domain.Contains(new Uri(configs.PermanentUrl).Host))
            {
                var merchantId = uri.Host.Split('.')[0];
                if (int.TryParse(merchantId, out int Id)) return Ok(ResponseUtility.CreateSuccessResopnse(new MerchantIdRM(Id)));
            }
            var promoteUrl = await _promoteUrlRepository.GetOneAsync(o => o.Url == $"{uri.Scheme}://{uri.Host}");
            if (promoteUrl != null) return Ok(ResponseUtility.CreateSuccessResopnse(new MerchantIdRM(promoteUrl.MerchantId)));
            return BadRequest();

        }
    }
}
