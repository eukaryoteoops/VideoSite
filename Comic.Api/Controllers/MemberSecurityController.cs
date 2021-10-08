using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Comic.Api.Commands.MemberSecurity;
using Comic.Cache;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberSecurityController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly int _memberId;
        private readonly ICache<Members> _memberCache;
        private readonly CacheTokenProvider _dscache;

        public MemberSecurityController(IMemberRepository memberRepository, IHttpContextAccessor ctx, ICache<Members> memberCache, CacheTokenProvider dscache)
        {
            _memberRepository = memberRepository;
            _memberId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "0");
            _memberCache = memberCache;
            _dscache = dscache;
        }

        #region 綁定
        /// <summary>
        ///     綁定裝置 - 發送 - 簡訊
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost("bind/sms")]
        public async ValueTask<IActionResult> SendBindingSms(SendBindingSms cmd)
        {
            var member = await _memberRepository.GetOneAsync(o => o.Phone == cmd.Phone);
            if (member != null) throw new Exception(ErrorCodes.PhoneDuplicated);
            var code = string.Join(string.Empty, Enumerable.Range(1, 9).OrderBy(o => new Random().Next()).Take(6));
            SendSms(cmd.Phone, code);
            await _dscache.SetStringAsync($"bindingSms_{_memberId}", $"{cmd.Phone}|{code}", TimeSpan.FromMinutes(10));
            return Ok();
        }

        /// <summary>
        ///     綁定裝置 - 發送 - Email
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost("bind/email")]
        public async ValueTask<IActionResult> SendBindingEmail(SendBindingEmail cmd)
        {
            var member = await _memberRepository.GetOneAsync(o => o.Email == cmd.Email);
            if (member != null) throw new Exception(ErrorCodes.EmailDuplicated);
            var code = string.Join(string.Empty, Enumerable.Range(1, 9).OrderBy(o => new Random().Next()).Take(6));
            await SendEmail(cmd.Email, code);
            await _dscache.SetStringAsync($"bindingEmail_{_memberId}", $"{cmd.Email}|{code}", TimeSpan.FromMinutes(10));
            return Ok();
        }

        /// <summary>
        ///     綁定裝置 - 驗證 - 簡訊
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost("bind/verify/sms")]
        public async ValueTask<IActionResult> VerifyBindingSms(VerifyBindingSms cmd)
        {
            var code = await _dscache.GetStringAsync($"bindingSms_{_memberId}");
            if (code == null) throw new Exception(ErrorCodes.VerifyCodeError);
            if (code.Split('|')[1] != cmd.Code) throw new Exception(ErrorCodes.VerifyCodeError);
            var member = await _memberRepository.GetOneAsync(o => o.Id == _memberId);
            member.UpdatePhone(code.Split('|')[0]);
            await _memberRepository.UpdateAsync(member);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{_memberId}");
            return Ok();
        }


        /// <summary>
        ///     綁定裝置 - 驗證 - Email
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost("bind/verify/email")]
        public async ValueTask<IActionResult> VerifyBindingEmail(VerifyBindingEmail cmd)
        {
            var code = await _dscache.GetStringAsync($"bindingEmail_{_memberId}");
            if (code == null) throw new Exception(ErrorCodes.VerifyCodeError);
            if (code.Split('|')[1] != cmd.Code) throw new Exception(ErrorCodes.VerifyCodeError);
            var member = await _memberRepository.GetOneAsync(o => o.Id == _memberId);
            member.UpdateEmail(code.Split('|')[0]);
            await _memberRepository.UpdateAsync(member);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{_memberId}");
            return Ok();
        }
        #endregion

        #region 找回
        /// <summary>
        ///     找回裝置 - 發送 - 簡訊
        /// </summary>
        /// <returns></returns>
        [HttpPost("shift/sms")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> SendShiftingSms(SendShiftingSms cmd)
        {
            var member = await _memberRepository.GetOneAsync(o => o.Phone == cmd.Phone);
            if (member == null) throw new Exception(ErrorCodes.PhoneNotExist);
            var code = string.Join(string.Empty, Enumerable.Range(1, 9).OrderBy(o => new Random().Next()).Take(6));
            SendSms(cmd.Phone, code);
            await _dscache.SetStringAsync($"shiftingSms_{cmd.Phone}", $"{member.Id}|{code}", TimeSpan.FromMinutes(10));
            return Ok();
        }

        /// <summary>
        ///     找回裝置 - 發送 - Email
        /// </summary>
        /// <returns></returns>
        [HttpPost("shift/email")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> SendShiftingEmail(SendShiftingEmail cmd)
        {
            var member = await _memberRepository.GetOneAsync(o => o.Email == cmd.Email);
            if (member == null) throw new Exception(ErrorCodes.EmailNotExist);
            var code = string.Join(string.Empty, Enumerable.Range(1, 9).OrderBy(o => new Random().Next()).Take(6));
            await SendEmail(cmd.Email, code);
            await _dscache.SetStringAsync($"shiftingEmail_{cmd.Email}", $"{member.Id}|{code}", TimeSpan.FromMinutes(10));
            return Ok();
        }

        /// <summary>
        ///     找回裝置 - 驗證 - 簡訊
        /// </summary>
        /// <returns></returns>
        [HttpPost("shift/verify/sms")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> VerifyShiftingSms(VerifyShiftingSms cmd)
        {
            var code = await _dscache.GetStringAsync($"shiftingSms_{cmd.Phone}");
            if (code == null) throw new Exception(ErrorCodes.VerifyCodeError);
            if (code.Split('|')[1] != cmd.Code) throw new Exception(ErrorCodes.VerifyCodeError);
            var memberId = Convert.ToInt32(code.Split('|')[0]);
            var member = await _memberRepository.GetOneAsync(o => o.Id == memberId);
            member.ChangeDevice(cmd.DeviceId);
            await _memberRepository.UpdateAsync(member);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{memberId}");
            return Ok(ResponseUtility.CreateSuccessResopnse(new { Hash = member.Hash }));
        }

        /// <summary>
        ///     找回裝置 - 驗證 - Email
        /// </summary>
        /// <returns></returns>
        [HttpPost("shift/verify/email")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> VerifyShiftingEmail(VerifyShiftingEmail cmd)
        {
            var code = await _dscache.GetStringAsync($"shiftingEmail_{cmd.Email}");
            if (code == null) throw new Exception(ErrorCodes.VerifyCodeError);
            if (code.Split('|')[1] != cmd.Code) throw new Exception(ErrorCodes.VerifyCodeError);
            var memberId = Convert.ToInt32(code.Split('|')[0]);
            var member = await _memberRepository.GetOneAsync(o => o.Id == memberId);
            member.ChangeDevice(cmd.DeviceId);
            await _memberRepository.UpdateAsync(member);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{memberId}");
            return Ok(ResponseUtility.CreateSuccessResopnse(new { Hash = member.Hash }));
        }

        /// <summary>
        ///     找回裝置 - 驗證 - 憑證
        /// </summary>
        /// <returns></returns>
        [HttpPost("shift/verify/certificate")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> VerifyShiftingCertificate(VerifyShiftingCertificate cmd)
        {
            var info = CryptographyUtility.RsaDecrypt(cmd.Hash).Split('|');
            var name = info[0];
            var member = await _memberRepository.GetOneAsync(o => o.Name == name && o.State);
            if (member == null)
                throw new Exception(ErrorCodes.AccountDisabled);
            if (CryptographyUtility.Validate(info[1], member.Salt, member.Password) == false)
                throw new Exception(ErrorCodes.AccountOrPwdError);
            member.ChangeDevice(cmd.DeviceId);
            await _memberRepository.UpdateAsync(member);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{member.Id}");
            return Ok();
        }
        #endregion

        #region Private

        private void SendSms(string phone, string code)
        {
            var secretKey = "lzlh_xlXaowaaNp1VGdqfzESC0D74aAxHYhrNaAA";
            var accessKey = "NTuNd_ct9eFMekGBrhIXgSDVfHKLsmg8-fBHF4Mu";
            var templateId = "1440867461007163392";
            var client = new RestClient("http://sms.qiniuapi.com");
            client.AddDefaultHeader("Host", "sms.qiniuapi.com");

            var request = new RestRequest("/v1/message/single", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = JsonSerializer.Serialize(new
            {
                template_id = templateId,
                mobile = phone,
                parameters = new Dictionary<string, string> { { "code", code } }
            });
            request.AddJsonBody(new { template_id = templateId, mobile = phone, parameters = new Dictionary<string, string> { { "code", code } } });
            var sb = new StringBuilder();
            sb.Append($"POST /v1/message/single");
            sb.Append($"\nHost: sms.qiniuapi.com");
            sb.Append($"\nContent-Type: application/json");
            sb.Append($"\n\n");
            sb.Append($"{body}");
            var auth = CryptographyUtility.HMACSHA1Encrypt(sb.ToString(), secretKey);
            request.AddHeader("Authorization", $"Qiniu {accessKey}:{auth}");
            var resp = client.Execute(request);
        }


        private async Task SendEmail(string email, string code)
        {
            var client = new SendGridClient("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");
            var msg = new SendGridMessage();
            msg.SetFrom("addtoonsite@gmail.com", "工口MH");
            msg.AddTo(email);
            msg.SetSubject("工口MH - 验证邮件");
            var content = $"Hi ,\n以下是您的邮箱验证码，请在网站 / APP上填写以通供验证\n(如果您从未请求发送邮箱验证码，请忽略此邮件)\n\n验证码: {code}";
            msg.AddContent(MimeType.Text, content);
            var resp = await client.SendEmailAsync(msg);
            var respContent = await resp.Body.ReadAsStringAsync();
        }
        #endregion
    }
}
