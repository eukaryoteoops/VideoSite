using System.Threading.Tasks;
using Comic.BackOffice.Commands.Config;
using Comic.BackOffice.ReadModels.Config;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Comic.BackOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly ICache<Configs> _configCache;
        private readonly IConfigRepository _configRepository;

        public ConfigController(ICache<Configs> configCache, IConfigRepository configRepository)
        {
            _configCache = configCache;
            _configRepository = configRepository;
        }

        [HttpGet()]
        [SwaggerResponse(typeof(ConfigRM))]
        public async Task<IActionResult> GetAll()
        {
            var config = await _configCache.GetOneAsync($"{CacheKeys.Configs}");
            return Ok(ResponseUtility.CreateSuccessResopnse(config.Adapt<ConfigRM>()));
        }

        [HttpGet("announcement")]
        [SwaggerResponse(typeof(AnnouncementRM))]
        public async Task<IActionResult> GetAnnouncements()
        {
            var config = await _configCache.GetOneAsync($"{CacheKeys.Configs}");
            return Ok(ResponseUtility.CreateSuccessResopnse(config.Adapt<AnnouncementRM>()));
        }

        [HttpPost("")]
        public async Task<IActionResult> UpdateConfigs(UpdateConfigs cmd)
        {
            var config = await _configCache.GetOneAsync($"{CacheKeys.Configs}");
            config.UpdateConfigs(cmd.IosVersion, cmd.IosUrl, cmd.IosBackupUrl, cmd.AndroidVersion, cmd.AndroidUrl, cmd.PermanentUrl, cmd.LatestUrl, cmd.SiteUrl, cmd.ImageUrl, cmd.ReleaseUrl, cmd.VideoDomain);
            await _configRepository.UpdateAsync(config);
            await _configCache.ClearAsync($"{CacheKeys.Configs}");
            return Ok();
        }

        [HttpPost("announcement/merchant")]
        public async Task<IActionResult> UpdateMerchantAnnouncement(UpdateMerchantAnnouncement cmd)
        {
            var config = await _configCache.GetOneAsync($"{CacheKeys.Configs}");
            config.UpdateMerchantAnnouncement(cmd.Content);
            await _configRepository.UpdateAsync(config);
            await _configCache.ClearAsync($"{CacheKeys.Configs}");
            return Ok();
        }

        [HttpPost("announcement/member")]
        public async Task<IActionResult> UpdateMemberAnnouncement(UpdateMemberAnnouncement cmd)
        {
            var config = await _configCache.GetOneAsync($"{CacheKeys.Configs}");
            config.UpdateMemberAnnouncement(cmd.Content);
            await _configRepository.UpdateAsync(config);
            await _configCache.ClearAsync($"{CacheKeys.Configs}");
            return Ok();
        }
    }
}
