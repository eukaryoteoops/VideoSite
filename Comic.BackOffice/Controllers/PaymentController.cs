using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Payment;
using Comic.BackOffice.QueryModels.Payment;
using Comic.BackOffice.ReadModels.Payment;
using Comic.Common.ExtensionMethods;
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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpGet]
        [SwaggerResponse(typeof(IEnumerable<PaymentRM>))]
        public async Task<IActionResult> GetAll([FromQuery] GetPayments qry)
        {
            Expression<Func<Payments, bool>> condition = o => true;
            List<Expression<Func<Payments, bool>>> lsExp = new List<Expression<Func<Payments, bool>>>();
            if (qry.Name != null)
                lsExp.Add(o => o.Name.Contains(qry.Name));
            if (qry.ChannelName != null)
                lsExp.Add(o => o.ChannelName.Contains(qry.ChannelName));
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var payments = await _paymentRepository.GetAsync(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(payments.Adapt<IEnumerable<PaymentRM>>()));
        }

        [HttpPost("state")]
        public async Task<IActionResult> UpdateState(UpdatePaymentState cmd)
        {
            var payment = await _paymentRepository.GetOneAsync(o => o.Id == cmd.Id);
            payment.UpdateState(cmd.State);
            await _paymentRepository.UpdateAsync(payment);
            return Ok();
        }

        [HttpPost("order")]
        public async Task<IActionResult> UpdateState(UpdatePaymentOrder cmd)
        {
            await _paymentRepository.UpdatepaymentOrder(cmd.PaymentIds);
            return Ok();
        }
    }
}
