using System;
using System.Threading.Tasks;
using Comic.Api.ReadModels.Advert;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("td")]
    [AllowAnonymous]
    public class AdvertController : ControllerBase
    {
        private readonly ICache<Adverts> _advertCache;

        public AdvertController(ICache<Adverts> advertCache)
        {
            _advertCache = advertCache;
        }

        /// <summary>
        ///     廣告資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(typeof(AdvertRM))]
        public async ValueTask<IActionResult> Get()
        {
            var adverts = await _advertCache.GetAsync($"{CacheKeys.Adverts}", null, TimeSpan.FromHours(1));
            return Ok(ResponseUtility.CreateSuccessResopnse(adverts.Adapt<AdvertRM>()));
        }
    }
}
