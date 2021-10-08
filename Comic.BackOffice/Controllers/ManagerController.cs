using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Manager;
using Comic.BackOffice.ReadModels.Manager;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Google.Authenticator;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static Comic.Common.BaseClasses.AppSettingsObject;

namespace Comic.BackOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerRepository _managerRepository;
        private readonly JwtObject _jwt;

        public ManagerController(IManagerRepository managerRepository, IOptions<JwtObject> opts)
        {
            _managerRepository = managerRepository;
            _jwt = opts.Value;
        }

        private async ValueTask<string> GenerateJwtToken(int userId, string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()),
                new Claim("name", userName),
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Login(Login cmd)
        {
            var Authenticator = new TwoFactorAuthenticator();
            if (!Authenticator.ValidateTwoFactorPIN("3edc$RFV", cmd.Code, TimeSpan.FromSeconds(30)))
                throw new Exception(ErrorCodes.AccountOrPwdError);
            var manager = await _managerRepository.GetOneAsync(o => o.State && o.Name == cmd.Name);
            if (manager == null)
                throw new Exception(ErrorCodes.AccountOrPwdError);
            if (CryptographyUtility.Validate(cmd.Password, manager.Salt, manager.Password) == false)
                throw new Exception(ErrorCodes.AccountOrPwdError);
            var token = await this.GenerateJwtToken(manager.Id, manager.Name);
            return Ok(ResponseUtility.CreateSuccessResopnse(new TokenRM(token)));
        }

        [HttpGet("check")]
        public async ValueTask<IActionResult> Check()
        {
            return Ok();
        }

        [HttpGet()]
        public async ValueTask<IActionResult> GetAll()
        {
            var managers = await _managerRepository.GetAsync();
            return Ok(ResponseUtility.CreateSuccessResopnse(managers.Adapt<IEnumerable<ManagerRM>>()));
        }

        [HttpPatch()]
        public async ValueTask<IActionResult> AddManager(AddManager cmd)
        {
            var manager = new Managers(cmd.Name, cmd.Password);
            try
            {
                await _managerRepository.AddAsync(manager);
            }
            catch
            {
                throw new Exception(ErrorCodes.AccountDuplicated);
            }
            return Ok();
        }

        [HttpPost()]
        public async ValueTask<IActionResult> UpdateManager(UpdateManager cmd)
        {
            var manager = await _managerRepository.GetOneAsync(o => o.Id == cmd.Id);
            manager.UpdateManager(cmd.Password);
            await _managerRepository.UpdateAsync(manager);
            return Ok();
        }

        [HttpPost("state")]
        public async Task<IActionResult> UpdateState(UpdateManagerState cmd)
        {
            var manager = await _managerRepository.GetOneAsync(o => o.Id == cmd.Id);
            manager.UpdateState(cmd.State);
            await _managerRepository.UpdateAsync(manager);
            return Ok();
        }

    }
}
