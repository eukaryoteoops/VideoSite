using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Comic.BackOffice.Merchant.Commands.Merchant;
using Comic.BackOffice.Merchant.ReadModels.Merchant;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag.Annotations;
using static Comic.Common.BaseClasses.AppSettingsObject;

namespace Comic.BackOffice.Merchant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPromoteUrlRepository _promoteUrlRepository;
        private readonly IConfigRepository _configRepository;
        private readonly JwtObject _jwt;
        private readonly int _merchantId;


        public MerchantController(IMerchantRepository merchantRepository, IPromoteUrlRepository promoteUrlRepository, IConfigRepository configRepository, IOptions<JwtObject> opts, IHttpContextAccessor ctx)
        {
            _merchantRepository = merchantRepository;
            _promoteUrlRepository = promoteUrlRepository;
            _configRepository = configRepository;
            _jwt = opts.Value;
            _merchantId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "0");
        }

        private async ValueTask<string> GenerateJwtToken(int userId, string userName, long createdTime)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()),
                new Claim("name", userName),
                new Claim("created_time", createdTime.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTimeOffset.Now.AddMinutes(_jwt.ExpiredIn);

            var token = new JwtSecurityToken(
                _jwt.Key,
                _jwt.Issuer,
                claims,
                null,
                expires.DateTime,
                creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("promote_urls")]
        [SwaggerResponse(typeof(PromoteUrlRM))]
        public async ValueTask<IActionResult> GetUrls()
        {
            var urls = await _promoteUrlRepository.GetAsync(o => o.MerchantId == _merchantId);
            var config = await _configRepository.GetOneAsync();
            return Ok(ResponseUtility.CreateSuccessResopnse(new PromoteUrlRM(config, urls, _merchantId)));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Login(Login cmd)
        {
            var merchant = await _merchantRepository.GetOneAsync(o => o.Name == cmd.Name);
            if (merchant == null)
                throw new Exception(ErrorCodes.AccountOrPwdError);
            if (CryptographyUtility.Validate(cmd.Password, merchant.Salt, merchant.Password) == false)
                throw new Exception(ErrorCodes.AccountOrPwdError);
            var token = await this.GenerateJwtToken(merchant.Id, merchant.Name, merchant.CreatedTime);
            return Ok(ResponseUtility.CreateSuccessResopnse(new TokenRM(token)));
        }

        [HttpGet("check")]
        public async ValueTask<IActionResult> Check()
        {
            return Ok();
        }

        [HttpPost("password")]
        public async ValueTask<IActionResult> UpdateMerchantPassword(UpdateMerchantPassword cmd)
        {
            var merchant = await _merchantRepository.GetOneAsync(o => o.Id == _merchantId);
            if (merchant == null)
                throw new Exception(ErrorCodes.AccountOrPwdError);
            if (!CryptographyUtility.Validate(cmd.OldPassword, merchant.Salt, merchant.Password))
                throw new Exception(ErrorCodes.AccountOrPwdError);
            merchant.UpdatePassword(cmd.NewPassword);
            await _merchantRepository.UpdateAsync(merchant);
            return Ok();
        }
    }
}
