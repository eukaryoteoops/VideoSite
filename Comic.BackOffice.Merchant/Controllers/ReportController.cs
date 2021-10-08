using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.BackOffice.Merchant.QueryModels.Report;
using Comic.BackOffice.Merchant.ReadModels.Report;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using static Comic.BackOffice.Merchant.ReadModels.Report.PerformanceReportRM;

namespace Comic.BackOffice.Merchant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly int _merchantId;
        private readonly IOrderRepository _orderRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMerchantSubRepository _merchantSubRepository;

        public ReportController(IOrderRepository orderRepository, IHttpContextAccessor ctx, IMerchantSubRepository merchantSubRepository, IMemberRepository memberRepository)
        {
            _orderRepository = orderRepository;
            _merchantSubRepository = merchantSubRepository;
            _memberRepository = memberRepository;
            _merchantId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "11004");
        }

        [HttpGet("performance")]
        [SwaggerResponse(typeof(PerformanceReportRM))]
        public async ValueTask<IActionResult> GetMerchantReport([FromQuery] GetPerformanceReport qry)
        {
            Expression<Func<Orders, bool>> condition = o => o.MerchantId == _merchantId && o.State;
            List<Expression<Func<Orders, bool>>> lsExp = new List<Expression<Func<Orders, bool>>>();
            lsExp.Add(o => o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var orders = await _orderRepository.GetAsync(condition);
            var result = new PerformanceReportRM(orders.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<Detail>>(), qry.PageNo, qry.PageSize);
            return Ok(ResponseUtility.CreateSuccessResopnse(result, qry.PageNo, qry.PageSize, result.Count));
        }

        [HttpGet("sub")]
        [SwaggerResponse(typeof(List<MerchantSubReportRM>))]
        public async ValueTask<IActionResult> GetSubReport([FromQuery] GetSubReport qry)
        {
            var subs = await _merchantSubRepository.GetAsync(o => o.ParentId == _merchantId);
            if (subs.Count() == 0) return Ok();
            var ids = subs.Select(o => o.ChildId);
            var members = await _memberRepository.GetAsync(o => ids.Contains(o.MerchantId) &&
            o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime && o.State);
            var orders = await _orderRepository.GetAsync(o => ids.Contains(o.MerchantId) &&
            o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime && o.State);
            var result = new List<MerchantSubReportRM>();
            orders.GroupBy(o => o.MerchantId).ToList().ForEach(o => result.Add(new MerchantSubReportRM(o.Key, o.ToList().FirstOrDefault().Merchant.Name, o.ToList())));
            result.ForEach(o => o.MemberCount = members.GroupBy(p => p.MerchantId).First(q => q.Key == o.Id).Count());
            if (result.Count() != subs.Count())
                foreach (var item in subs)
                    if (!result.Any(o => o.Id == item.Merchants.Id))
                        result.Add(new MerchantSubReportRM() { Id = item.Merchants.Id, Name = item.Merchants.Name });
            result = result.OrderBy(o => o.Id).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(result));

        }
    }
}
