using System;
using System.Linq;
using System.Threading.Tasks;
using Comic.BackOffice.Merchant.ReadModels.Home;
using Comic.Common.Utilities;
using Comic.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Comic.BackOffice.Merchant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConfigRepository _configRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly int _merchantId;

        public HomeController(IConfigRepository configRepository, IOrderRepository orderRepository, IMemberRepository memberRepository, IHttpContextAccessor ctx)
        {
            _configRepository = configRepository;
            _orderRepository = orderRepository;
            _memberRepository = memberRepository;
            _merchantId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "11011");
        }

        [HttpGet("statistic")]
        [SwaggerResponse(typeof(StatisticRM))]
        public async ValueTask<IActionResult> GetStatistic()
        {
            var members = _memberRepository.GetAsync(o => o.MerchantId == _merchantId);
            var orders = _orderRepository.GetAsync(o => o.MerchantId == _merchantId && o.State == true);
            return Ok(ResponseUtility.CreateSuccessResopnse(new StatisticRM(await members, await orders)));
        }

        [HttpGet("announcement")]
        [SwaggerResponse(typeof(AnnouncementRM))]
        public async ValueTask<IActionResult> GetAnnouncement()
        {
            var config = await _configRepository.GetOneAsync();
            return Ok(ResponseUtility.CreateSuccessResopnse(new AnnouncementRM(config.MerchantAnnouncement)));
        }
    }
}
