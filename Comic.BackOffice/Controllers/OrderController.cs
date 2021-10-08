using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Order;
using Comic.BackOffice.QueryModels.Order;
using Comic.BackOffice.ReadModels.Order;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Comic.BackOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICache<Members> _memberCache;
        private readonly ICache<Orders> _OrderCache;

        public OrderController(IOrderRepository orderRepository, ICache<Members> memberCache, ICache<Orders> orderCache)
        {
            _orderRepository = orderRepository;
            _memberCache = memberCache;
            _OrderCache = orderCache;
        }

        [HttpGet()]
        [SwaggerResponse(typeof(IEnumerable<OrderRM>))]
        public async ValueTask<IActionResult> GetAll([FromQuery] GetOrders qry)
        {
            Expression<Func<Orders, bool>> condition = o => true;
            List<Expression<Func<Orders, bool>>> lsExp = new List<Expression<Func<Orders, bool>>>();
            if (qry.Name != null)
                lsExp.Add(o => o.Member.Name.Contains(qry.Name));
            if (qry.ChannelName != null)
                lsExp.Add(o => o.Payment.ChannelName.Contains(qry.ChannelName));
            if (qry.State != null)
                lsExp.Add(o => o.State == qry.State);
            if (qry.StartTime != null || qry.EndTime != null)
                lsExp.Add(o => o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var orders = await _orderRepository.GetWithSortingAsync(condition, "CreatedTime Desc", qry.PageNo, qry.PageSize);
            var amount = await _orderRepository.GetAmount(condition);

            return Ok(ResponseUtility.CreateSuccessResopnse(orders.Adapt<IEnumerable<OrderRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        [HttpPost("state")]
        public async ValueTask<IActionResult> UpdateOrder(UpdateOrderState cmd)
        {
            var order = await _orderRepository.GetOneAsync(o => o.OrderId == cmd.OrderId);
            if (order.State) return Ok();
            await _orderRepository.OrderSuccess(order);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{order.MemberId}");
            await _OrderCache.ClearAsync($"{CacheKeys.MemberValidOrders}{order.MemberId}");
            return Ok();
        }

        [HttpPost("change_merchant")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> ChangeOrderMerchant(ChangeOrderMerchant cmd)
        {
            var order = await _orderRepository.GetOneAsync(o => o.OrderId == cmd.OrderId);
            order.MerchantId = cmd.MerchantId;
            order.MerchantBonus = cmd.MerchantBonus;
            await _orderRepository.UpdateAsync(order);
            return Ok();

        }
    }
    public class ChangeOrderMerchant
    {
        public string OrderId { get; set; }
        public int MerchantId { get; set; }
        public int MerchantBonus { get; set; }
    }
}
