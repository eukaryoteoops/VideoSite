using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Merchant;
using Comic.BackOffice.QueryModels.Merchant;
using Comic.BackOffice.ReadModels.Merchant;
using Comic.Common.BaseClasses;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using static Comic.BackOffice.ReadModels.Merchant.MerchantReportRM;

namespace Comic.BackOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IMerchantSubRepository _merchantSubRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPromoteUrlRepository _promoteUrlRepository;
        private readonly IMemberRepository _memberRepository;

        public MerchantController(IMerchantRepository merchantRepository, IOrderRepository orderRepository, IPromoteUrlRepository promoteUrlRepository, IMemberRepository memberRepository, IMerchantSubRepository merchantSubRepository)
        {
            _merchantRepository = merchantRepository;
            _orderRepository = orderRepository;
            _promoteUrlRepository = promoteUrlRepository;
            _memberRepository = memberRepository;
            _merchantSubRepository = merchantSubRepository;
        }

        [HttpGet()]
        [SwaggerResponse(typeof(IEnumerable<MerchantRM>))]
        public async ValueTask<IActionResult> GetAll([FromQuery] GetMerchants qry)
        {
            Expression<Func<Merchants, bool>> condition = o => true;
            List<Expression<Func<Merchants, bool>>> lsExp = new List<Expression<Func<Merchants, bool>>>();
            if (qry.Id != null)
                lsExp.Add(o => o.Id.ToString().Contains(qry.Id));
            if (qry.Name != null)
                lsExp.Add(o => o.Name.Contains(qry.Name));
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var merchants = await _merchantRepository.GetWithSortingAsync(condition, "CreatedTime Desc", qry.PageNo, qry.PageSize);
            var amount = await _merchantRepository.GetAmount(condition);

            return Ok(ResponseUtility.CreateSuccessResopnse(merchants.Adapt<IEnumerable<MerchantRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("sub")]
        [SwaggerResponse(typeof(IEnumerable<MerchantSubRM>))]
        public async ValueTask<IActionResult> GetSub([FromQuery] int parentId)
        {
            Expression<Func<MerchantSubs, bool>> condition = o => true;
            List<Expression<Func<MerchantSubs, bool>>> lsExp = new List<Expression<Func<MerchantSubs, bool>>>();
            lsExp.Add(o => o.ParentId == parentId);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var childs = await _merchantSubRepository.GetAsync(condition);
            if (childs.Count() == 0) return Ok(ResponseUtility.CreateSuccessResopnse());

            return Ok(ResponseUtility.CreateSuccessResopnse(childs.Select(o => o.Merchants).Adapt<IEnumerable<MerchantSubRM>>()));
        }

        [HttpGet("url")]
        [SwaggerResponse(typeof(IEnumerable<PromoteUrlRM>))]
        public async ValueTask<IActionResult> GetPromoteUrl([FromQuery] GetPromoteUrl qry)
        {
            Expression<Func<PromoteUrls, bool>> condition = o => true;
            List<Expression<Func<PromoteUrls, bool>>> lsExp = new List<Expression<Func<PromoteUrls, bool>>>();
            lsExp.Add(o => o.MerchantId == qry.Id);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var urls = await _promoteUrlRepository.GetAsync(condition);

            return Ok(ResponseUtility.CreateSuccessResopnse(urls.Adapt<IEnumerable<PromoteUrlRM>>()));
        }

        [HttpGet("report/merchant")]
        [SwaggerResponse(typeof(MerchantReportRM))]
        public async ValueTask<IActionResult> GetMerchantReport([FromQuery] GetMerchantReport qry)
        {
            Expression<Func<Orders, bool>> condition = o => o.State == true;
            List<Expression<Func<Orders, bool>>> lsExp = new List<Expression<Func<Orders, bool>>>();
            if (qry.Id != null)
                lsExp.Add(o => o.Merchant.Id == qry.Id);
            if (qry.Name != null)
                lsExp.Add(o => o.Merchant.Name.Contains(qry.Name));
            if (qry.StartTime != null || qry.EndTime != null)
                lsExp.Add(o => o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var orders = await _orderRepository.GetAsync(condition);
            var result = new MerchantReportRM(orders.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<Detail>>(), qry.PageNo, qry.PageSize);
            return Ok(ResponseUtility.CreateSuccessResopnse(result, qry.PageNo, qry.PageSize, result.Count));
        }

        [HttpGet("report/platform")]
        [SwaggerResponse(typeof(IEnumerable<PlatformReportRM>))]
        public async ValueTask<IActionResult> GetPlatformReport([FromQuery] GetPlatformReport qry)
        {
            var orders = await _orderRepository.GetAsync(o => o.State && o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            var members = await _memberRepository.GetAsync(o => o.State && o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            var result = orders.GroupBy(o => DateTimeOffset.FromUnixTimeSeconds(o.CreatedTime).ToOffset(TimeSpan.FromHours(8)).Date).OrderByDescending(o => o.Key).Select(o => new PlatformReportRM
            {
                Date = o.Key,
                MemberCount = members.Where(x => DateTimeOffset.FromUnixTimeSeconds(x.CreatedTime).ToOffset(TimeSpan.FromHours(8)).Date == o.Key).Count(),
                OrderCount = o.Count(),
                Amount = o.Sum(x => x.Product.Price)
            });
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        [HttpGet("report/platform/detail")]
        [SwaggerResponse(typeof(IEnumerable<PlatformReportDeatilRM>))]
        public async ValueTask<IActionResult> GetPlatformReportDetail([FromQuery] GetPlatformReportDetail qry)
        {
            var merchants = await _merchantRepository.GetAsync(o => o.State);
            var orders = await _orderRepository.GetAsync(o => o.State && o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            var members = await _memberRepository.GetAsync(o => o.State && o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            var result = merchants.Select(o => o.Id).OrderBy(o => o).Select(o => new PlatformReportDeatilRM(o, orders.Where(p => p.MerchantId == o).ToList(), members.Where(p => p.MerchantId == o).ToList()));
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }


        [HttpPatch()]
        public async ValueTask<IActionResult> AddMerchant(AddMerchant cmd)
        {
            var merchant = new Merchants(cmd.Name, cmd.NickName, cmd.Bonus);
            try
            {
                await _merchantRepository.AddAsync(merchant);
            }
            catch
            {
                throw new Exception(ErrorCodes.AccountDuplicated);
            }
            return Ok();
        }

        [HttpPost("url")]
        public async ValueTask<IActionResult> UpdatePromoteUrls(UpdatePromoteUrls cmd)
        {
            await _promoteUrlRepository.UpdatePromoteUrls(cmd.Id, cmd.Urls);
            return Ok();
        }

        [OpenApiIgnore]
        [HttpPost("sub")]
        public async ValueTask<IActionResult> UpdateSubs(UpdateSubs cmd)
        {
            await _merchantSubRepository.UpdateSubs(cmd.ParentId, cmd.ChildrenIds);
            return Ok();
        }

        [HttpPost()]
        public async ValueTask<IActionResult> UpdateMerchant(UpdateMerchant cmd)
        {
            var merchant = await _merchantRepository.GetOneAsync(o => o.Id == cmd.Id);
            merchant.UpdateMerchant(cmd.NickName, cmd.Bonus);
            await _merchantRepository.UpdateAsync(merchant);
            return Ok();
        }

        [HttpPost("state")]
        public async Task<IActionResult> UpdateState(UpdateMerchantState cmd)
        {
            var product = await _merchantRepository.GetOneAsync(o => o.Id == cmd.Id);
            product.UpdateState(cmd.State);
            await _merchantRepository.UpdateAsync(product);
            return Ok();
        }

        [HttpPost("reset")]
        public async ValueTask<IActionResult> ResetPassword(ResetMerchantPassword cmd)
        {
            var merchant = await _merchantRepository.GetOneAsync(o => o.Id == cmd.Id);
            var password = RandomStringUtility.Create(8);
            merchant.UpdatePassword(password);
            await _merchantRepository.UpdateAsync(merchant);
            return Ok(ResponseUtility.CreateSuccessResopnse(new ResetPasswordRM() { Password = password }));
        }
    }
}
