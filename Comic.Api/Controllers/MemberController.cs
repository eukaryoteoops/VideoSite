using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Comic.Api.Commands.Member;
using Comic.Api.ReadModels.Member;
using Comic.Cache;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Fare;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag.Annotations;
using static Comic.Common.BaseClasses.AppSettingsObject;

namespace Comic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IComicFavoriteRepository _comicFavoriteRepository;
        private readonly IVideoFavoriteRepository _videoFavoriteRepository;
        private readonly IChapterPurchaseRepository _chapterPurchaseRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IComicHistoryRepository _comicHistoryRepository;
        private readonly IPointJournalRepository _pointJournalRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly JwtObject _jwt;
        private readonly int _memberId;
        private readonly HttpContext _ctx;
        private readonly ICache<Members> _memberCache;
        private readonly ICache<Orders> _orderCache;
        private readonly CacheTokenProvider _dscache;
        private readonly IMemoryCache _memoryCache;

        public MemberController(IMemberRepository memberRepository, IComicFavoriteRepository favoriteRepository, IChapterPurchaseRepository chapterPurchaseRepository, IChapterRepository chapterRepository, IComicHistoryRepository readingHistoryRepository, IPointJournalRepository pointJournalRepository, IMerchantRepository merchantRepository, IOptions<JwtObject> opts, IHttpContextAccessor ctx, IVideoFavoriteRepository videoFavoriteRepository, ICache<Members> memberCache, CacheTokenProvider dscache, ICache<Orders> orderCache, IMemoryCache memoryCache)
        {
            _memberRepository = memberRepository;
            _comicFavoriteRepository = favoriteRepository;
            _chapterPurchaseRepository = chapterPurchaseRepository;
            _chapterRepository = chapterRepository;
            _comicHistoryRepository = readingHistoryRepository;
            _pointJournalRepository = pointJournalRepository;
            _merchantRepository = merchantRepository;
            _jwt = opts.Value;
            _memberId = Convert.ToInt32(ctx.HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "1026272");
            _ctx = ctx.HttpContext;
            _videoFavoriteRepository = videoFavoriteRepository;
            _memberCache = memberCache;
            _orderCache = orderCache;
            _dscache = dscache;
            _memoryCache = memoryCache;
        }

        /// <summary>
        ///     用戶資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [SwaggerResponse(typeof(MemberRM))]
        public async ValueTask<IActionResult> GetInfo()
        {
            var member = await _memberCache.GetOneAsync($"{CacheKeys.SingleMember}{_memberId}", o => o.Id == _memberId && o.State, TimeSpan.FromHours(1));
            return Ok(ResponseUtility.CreateSuccessResopnse(member.Adapt<MemberRM>()));
        }

        [HttpPost("login")]
        [SwaggerResponse(typeof(TokenRM))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Login(Login cmd)
        {
            var info = CryptographyUtility.RsaDecrypt(cmd.Hash).Split('|');
            var name = info[0];
            var member = await _memberRepository.GetOneAsync(o => o.Name == name && o.State);
            if (member == null)
                throw new Exception(ErrorCodes.AccountDisabled);
            if (CryptographyUtility.Validate(info[1], member.Salt, member.Password) == false)
                throw new Exception(ErrorCodes.AccountOrPwdError);
            if (member.DeviceId != cmd.DeviceId)
                throw new Exception(ErrorCodes.UnknownDevice);
            var token = await this.GenerateJwtToken(member.Id, member.Name);
            member.UpdateLoginTime();
            await _memberRepository.UpdateAsync(member);
            _memoryCache.Set<string>($"{CacheKeys.MemberToken}{member.Id}", token, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(6) });
            //await _dscache.SetStringAsync($"{CacheKeys.MemberToken}{member.Id}", token);
            return Ok(ResponseUtility.CreateSuccessResopnse(new TokenRM(token)));
        }

        /// <summary>
        ///     註冊 Web
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [SwaggerResponse(typeof(RegisterRM))]
        [AllowAnonymous]
        public async ValueTask<IActionResult> Register(Register cmd)
        {
            var name = string.Empty;
            do
            {
                var nameReg = new Xeger("[23456789ABCDEFGHJKLMNPQRSTUVWXYZ]{8}");
                name = nameReg.Generate();
            } while (await _memberRepository.GetOneAsync(o => o.Name == name) != null);
            var pwdReg = new Xeger("[0-9a-zA-Z]{8}");
            var pwd = pwdReg.Generate();
            var hash = CryptographyUtility.RsaEncrypt($"{name}|{pwd}");
            if (cmd.MerchantId.HasValue)
            {
                var merchant = await _merchantRepository.GetOneAsync(o => o.Id == cmd.MerchantId.Value);
                if (merchant == null) cmd.MerchantId = null;
            }
            var newMember = new Members();
            do
            {
                newMember = new Members(name, pwd, hash, cmd.DeviceId, cmd.MerchantId);
            } while (await _memberRepository.GetOneAsync(o => o.Id == newMember.Id) != null);
            await _memberRepository.AddAsync(newMember);
            return Ok(ResponseUtility.CreateSuccessResopnse(new RegisterRM(name, hash)));
        }

        /// <summary>
        ///     發布頁紀錄IP
        /// </summary>
        /// <returns></returns>
        [HttpPost("pre_register")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> PreRegister(int Id)
        {
            var ip = _ctx.Request.Headers["X-Real-IP"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0];
            if (ip == "::1") return Ok();
            await _dscache.SetStringAsync(ip, Id.ToString(), TimeSpan.FromHours(1));
            return Ok();
        }

        /// <summary>
        ///     註冊 APP
        /// </summary>
        /// <returns></returns>
        [HttpPost("register_app")]
        [AllowAnonymous]
        public async ValueTask<IActionResult> RegisterApp(Register cmd)
        {
            var name = string.Empty;
            do
            {
                var nameReg = new Xeger("[23456789ABCDEFGHJKLMNPQRSTUVWXYZ]{8}");
                name = nameReg.Generate();
            } while (await _memberRepository.GetOneAsync(o => o.Name == name) != null);
            var pwdReg = new Xeger("[0-9a-zA-Z]{8}");
            var pwd = pwdReg.Generate();
            var hash = CryptographyUtility.RsaEncrypt($"{name}|{pwd}");
            var member = await _memberRepository.GetOneAsync(o => o.Name == name && o.State);
            if (member != null)
                throw new Exception(ErrorCodes.AccountDuplicated);
            int? merchantId = null;
            if (cmd.MerchantId != null) merchantId = cmd.MerchantId;
            else
            {
                var ip = _ctx.Request.Headers["X-Real-IP"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0];
                var merchantIdStr = await _dscache.GetStringAsync(ip);
                if (merchantIdStr != null)
                {
                    merchantId = Convert.ToInt32(merchantIdStr);
                    var merchant = await _merchantRepository.GetOneAsync(o => o.Id == merchantId);
                    if (merchant == null) merchantId = null;
                }
            }
            var newMember = new Members();
            do
            {
                newMember = new Members(name, pwd, hash, cmd.DeviceId, merchantId, cmd.Source ?? "unknown");
            } while (await _memberRepository.GetOneAsync(o => o.Id == newMember.Id) != null);
            await _memberRepository.AddAsync(newMember);
            return Ok(ResponseUtility.CreateSuccessResopnse(new RegisterRM(name, hash)));
        }

        [HttpGet("order")]
        [SwaggerResponse(typeof(IEnumerable<MemberOrderRM>))]
        public async ValueTask<IActionResult> GetOrders()
        {
            var orders = await _orderCache.GetAsync($"{CacheKeys.MemberValidOrders}{_memberId}", o => o.MemberId == _memberId && o.State);
            return Ok(ResponseUtility.CreateSuccessResopnse(orders.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<MemberOrderRM>>()));
        }


        [HttpGet("point")]
        [SwaggerResponse(typeof(IEnumerable<MemberPointJournalRM>))]
        public async ValueTask<IActionResult> GetPointJournals()
        {
            var pointJournals = await _pointJournalRepository.GetAsync(o => o.MemberId == _memberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(pointJournals.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<MemberPointJournalRM>>()));
        }

        /// <summary>
        ///     漫畫頁會員相關資訊
        /// </summary>
        /// <param name="comicId"></param>
        /// <returns></returns>
        [HttpGet("comic/{comicId}")]
        [SwaggerResponse(typeof(ComicInfoRM))]
        public async ValueTask<IActionResult> GetInfoByComicId([FromRoute] int comicId)
        {
            var member = await _memberCache.GetOneAsync($"{CacheKeys.SingleMember}{_memberId}", o => o.Id == _memberId && o.State);
            var favorite = await _comicFavoriteRepository.GetOneAsync(o => o.MemberId == _memberId && o.ComicId == comicId);
            var reading = await _comicHistoryRepository.GetOneAsync(o => o.MemberId == _memberId && o.ComicId == comicId);
            var chapterPurchase = await _chapterPurchaseRepository.GetAsync(o => o.MemberId == _memberId && o.ComicId == comicId);
            return Ok(ResponseUtility.CreateSuccessResopnse(new ComicInfoRM(member, favorite, reading, chapterPurchase)));
        }

        /// <summary>
        ///     視頻頁會員相關資訊
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [HttpGet("video/{cid}")]
        [SwaggerResponse(typeof(ComicInfoRM))]
        public async ValueTask<IActionResult> GetInfoByVideoId([FromRoute] string cid)
        {
            var member = await _memberCache.GetOneAsync($"{CacheKeys.SingleMember}{_memberId}", o => o.Id == _memberId && o.State);
            var favorite = await _videoFavoriteRepository.GetOneAsync(o => o.MemberId == _memberId && o.Cid == cid);
            return Ok(ResponseUtility.CreateSuccessResopnse(new VideoInfoRM(member, favorite)));
        }

        /// <summary>
        ///     章節頁檢查
        /// </summary>
        /// <param name="comicId"></param>
        /// <param name="chapterNumber"></param>
        /// <returns></returns>
        [HttpPatch("comic/{comicId}/{chapterNumber}")]
        public async ValueTask<IActionResult> CheckChapter([FromRoute] int comicId, [FromRoute] int chapterNumber)
        {
            // 0. free chapter
            var chapter = await _chapterRepository.GetOneAsync(o => o.ComicId == comicId && o.Number == chapterNumber);
            if (chapter == null) throw new Exception(ErrorCodes.ChapterNotExist);
            if (chapter.Point == 0) return Ok(ResponseUtility.CreateSuccessResopnse(new ChapterCountRM(chapter.Count, chapter.Title)));
            // 1. vip check
            var member = await _memberCache.GetOneAsync($"{CacheKeys.SingleMember}{_memberId}", o => o.Id == _memberId && o.State);
            if (member.IsPremium) return Ok(ResponseUtility.CreateSuccessResopnse(new ChapterCountRM(chapter.Count, chapter.Title)));
            if (member.PackageTime >= DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return Ok(ResponseUtility.CreateSuccessResopnse(new ChapterCountRM(chapter.Count, chapter.Title)));
            // 2. chapter purchase check
            var chapterPurchase = await _chapterPurchaseRepository.GetOneAsync(o => o.MemberId == _memberId && o.ComicId == comicId && o.ChapterNumber == chapterNumber);
            if (chapterPurchase != null) return Ok(ResponseUtility.CreateSuccessResopnse(new ChapterCountRM(chapter.Count, chapter.Title)));
            // 3. buy chapter implicit
            if (member.Point < chapter.Point) throw new Exception(ErrorCodes.NotEnougnMoney);
            await _chapterPurchaseRepository.PurchaseChapter(member, chapter);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{_memberId}");
            return Ok(ResponseUtility.CreateSuccessResopnse(new ChapterCountRM(chapter.Count, chapter.Title)));
        }

        #region Private

        private async ValueTask<string> GenerateJwtToken(int memberId, string memberName)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, memberId.ToString()),
                new Claim("name", memberName),
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

        //[HttpGet("test")]
        //public async ValueTask<IActionResult> Test()
        //{
        //    SendSms("18411036772", "123455");
        //    return Ok(ResponseUtility.CreateSuccessResopnse());
        //}

        #endregion
    }
}
