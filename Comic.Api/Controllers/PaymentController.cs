using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Comic.Api.QueryModels.Payment;
using Comic.Api.ReadModels.Payment;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.ExtensionMethods;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Serilog;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IProductDefaultConfigsRepository _productDefaultConfigsRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly int _memberId;
        private readonly ICache<Members> _memberCache;
        private readonly ICache<Orders> _OrderCache;


        public PaymentController(IProductRepository productRepository, IPaymentRepository paymentRepositor, IMerchantRepository merchantRepository, IOrderRepository orderRepository, IHttpContextAccessor ctx, IProductDefaultConfigsRepository productDefaultConfigsRepository, ICache<Members> dscache, ICache<Orders> orderCache)
        {
            _productRepository = productRepository;
            _paymentRepository = paymentRepositor;
            _merchantRepository = merchantRepository;
            _orderRepository = orderRepository;
            _memberId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "0");
            _productDefaultConfigsRepository = productDefaultConfigsRepository;
            _memberCache = dscache;
            _OrderCache = orderCache;
        }

        /// <summary>
        ///     取得產品
        /// </summary>
        /// <returns></returns>
        [HttpGet("product")]
        [SwaggerResponse(typeof(IEnumerable<ProductRM>))]
        public async ValueTask<IActionResult> GetProducts([FromQuery] GetProduct qry)
        {
            var products = await _productRepository.GetAsync(o => o.State == 1 && o.Type == qry.Type);
            var productDefault = await _productDefaultConfigsRepository.GetOneAsync(o => o.Type == qry.Type);
            var result = products.OrderBy(o => o.Order).Adapt<IEnumerable<ProductRM>>();
            result = result.Select(o =>
            {
                if (o.Id == productDefault.ProductId)
                    o.IsDefault = true;
                return o;
            }).ToList();
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        /// <summary>
        ///     取得支付渠道
        /// </summary>
        /// <returns></returns>
        [HttpGet("channel")]
        [SwaggerResponse(typeof(IEnumerable<ChannelRM>))]
        public async ValueTask<IActionResult> GetChannels([FromQuery] GetChannels qry)
        {
            var product = await _productRepository.GetOneAsync(o => o.Id == qry.ProductId);
            Expression<Func<Payments, bool>> condition = o => o.State == true && o.MinAmount <= product.Price && o.MaxAmount >= product.Price;
            List<Expression<Func<Payments, bool>>> lsExp = new List<Expression<Func<Payments, bool>>>();
            if (qry.Device != 0)
                lsExp.Add(o => o.Device == qry.Device || o.Device == 0);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var channels = await _paymentRepository.GetAsync(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(channels.OrderBy(o => o.Order).Adapt<IEnumerable<ChannelRM>>()));
        }

        /// <summary>
        ///     取得支付連結
        /// </summary>
        /// <returns></returns>
        [HttpGet("order")]
        public async ValueTask<IActionResult> GetPaymentUrl([FromQuery] GetPaymentUrl qry)
        {
            var product = await _productRepository.GetOneAsync(o => o.Id == qry.ProductId && o.State == 1);
            var payment = await _paymentRepository.GetOneAsync(o => o.Id == qry.PaymentId);
            var member = await _memberCache.GetOneAsync($"{CacheKeys.SingleMember}{_memberId}", o => o.Id == _memberId && o.State);
            var merchant = await _merchantRepository.GetOneAsync(o => o.Id == member.MerchantId);
            var newOrder = new Orders(_memberId, qry.PaymentId, qry.ProductId, merchant.Id, merchant.Bonus);
            var order = await _orderRepository.AddAsync(newOrder);
            var key = "XIsdizDJ";
            var clientId = 1008;
            var callbackUrl = "https://api.aoaotoon.com/payment/callback";
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes($"Amount={product.Price}&CallbackUrl={callbackUrl}&ChannelId={payment.Code}&ClientId={clientId}&OrderId={order.OrderId}&ReturnUrl={qry.ReturnUrl}&Key={key}"));
                var sign = BitConverter.ToString(hash).Replace("-", string.Empty);
                return Ok($"http://a.rc168c.com/payment?Amount={product.Price}&CallbackUrl={callbackUrl}&ChannelId={payment.Code}&ClientId={clientId}&OrderId={order.OrderId}&ReturnUrl={qry.ReturnUrl}&Sign={sign}");
            }
        }


        /// <summary>
        ///     Callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("callback")]
        [AllowAnonymous]
        [OpenApiIgnore]
        public async ValueTask<IActionResult> Callback([FromQuery] GetCallback qry)
        {
            Log.Information($"{qry.OrderId}|{qry.Amount}|{qry.Sign}");
            var key = "XIsdizDJ";
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes($"Amount={qry.Amount}&OrderId={qry.OrderId}&Key={key}"));
                var sign = BitConverter.ToString(hash).Replace("-", string.Empty);
                if (sign != qry.Sign) return BadRequest();
            }
            var order = await _orderRepository.GetOneAsync(o => o.OrderId == qry.OrderId);
            if (order.State) return Content("OK");
            await _orderRepository.OrderSuccess(order);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{order.MemberId}");
            await _OrderCache.ClearAsync($"{CacheKeys.MemberValidOrders}{order.MemberId}");
            return Content("OK");
        }
    }
}
