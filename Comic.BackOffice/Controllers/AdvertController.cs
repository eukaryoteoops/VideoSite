using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Advert;
using Comic.BackOffice.QueryModels.Advert;
using Comic.BackOffice.ReadModels.Advert;
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
    [Route("td")]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly ICache<Adverts> _advertCache;


        public AdvertController(IAdvertRepository advertRepository, ICache<Adverts> advertCache)
        {
            _advertRepository = advertRepository;
            _advertCache = advertCache;
        }

        [HttpGet()]
        [SwaggerResponse(typeof(IEnumerable<AdvertRM>))]
        public async Task<IActionResult> GetAll([FromQuery] GetAdverts qry)
        {
            var adverts = await _advertRepository.GetAsync(o => o.Type == qry.Type);
            return Ok(ResponseUtility.CreateSuccessResopnse(adverts.Adapt<IEnumerable<AdvertRM>>()));
        }

        [HttpPatch()]
        public async ValueTask<IActionResult> AddAdvert(AddAdvert cmd)
        {
            var advert = new Adverts(cmd.Type, cmd.Pic, cmd.Url);
            await _advertRepository.AddAsync(advert);
            await _advertCache.ClearAsync($"{CacheKeys.Adverts}");
            return Ok();
        }

        [HttpPost("order")]
        public async Task<IActionResult> UpdateOrder(UpdateAdvertOrder cmd)
        {
            await _advertRepository.UpdateAdvertOrder(cmd.AdvertIds);
            await _advertCache.ClearAsync($"{CacheKeys.Adverts}");
            return Ok();
        }

        [HttpPost()]
        public async ValueTask<IActionResult> UpdateAdvert(UpdateAdvert cmd)
        {
            var advert = await _advertRepository.GetOneAsync(o => o.Id == cmd.Id);
            advert.UpdateAdvert(cmd.Pic, cmd.Url);
            await _advertRepository.UpdateAsync(advert);
            await _advertCache.ClearAsync($"{CacheKeys.Adverts}");
            return Ok();
        }

        [HttpDelete()]
        public async ValueTask<IActionResult> DeleteAdvert(DeleteAdvert cmd)
        {
            var advert = await _advertRepository.GetOneAsync(o => o.Id == cmd.Id);
            await _advertRepository.DeleteAsync(advert);
            await _advertCache.ClearAsync($"{CacheKeys.Adverts}");
            return Ok();
        }
    }
}
