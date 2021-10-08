using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Member;
using Comic.BackOffice.QueryModels.Member;
using Comic.BackOffice.ReadModels.Member;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
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
    public class MemberController : ControllerBase
    {

        private readonly IMemberRepository _memberRepository;
        private readonly IPointJournalRepository _pointJournalRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICompensationJournalRepository _compensationJournalRepository;
        private readonly IComicFavoriteRepository _comicFavoriteRepository;
        private readonly IComicHistoryRepository _comicHistoryRepository;
        private readonly ICache<Members> _memberCache;

        public MemberController(IMemberRepository memberRepository, IPointJournalRepository pointJournalRepository, IOrderRepository orderRepository, ICompensationJournalRepository compensationJournalRepository, IComicFavoriteRepository comicFavoriteRepository, IComicHistoryRepository comicHistoryRepository, ICache<Members> memberCache)
        {
            _memberRepository = memberRepository;
            _pointJournalRepository = pointJournalRepository;
            _orderRepository = orderRepository;
            _compensationJournalRepository = compensationJournalRepository;
            _comicFavoriteRepository = comicFavoriteRepository;
            _comicHistoryRepository = comicHistoryRepository;
            _memberCache = memberCache;
        }

        [HttpGet()]
        [SwaggerResponse(typeof(IEnumerable<MemberRM>))]
        public async ValueTask<IActionResult> GetAll([FromQuery] GetMembers qry)
        {
            Expression<Func<Members, bool>> condition = o => true;
            List<Expression<Func<Members, bool>>> lsExp = new List<Expression<Func<Members, bool>>>();
            if (qry.Name != null)
                lsExp.Add(o => o.Name.Contains(qry.Name));
            if (qry.Phone != null)
                lsExp.Add(o => o.Phone.Contains(qry.Phone));
            if (qry.Email != null)
                lsExp.Add(o => o.Email.Contains(qry.Email));
            if (qry.VipType == 1)
                lsExp.Add(o => o.PackageTime > DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (qry.VipType == 2)
                lsExp.Add(o => o.IsPremium);
            if (qry.MerchantId != null)
                lsExp.Add(o => o.MerchantId == qry.MerchantId);
            if (qry.Source != null)
                lsExp.Add(o => o.Source == qry.Source);
            if (qry.StartTime != null || qry.EndTime != null)
                lsExp.Add(o => o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var members = await _memberRepository.GetWithSortingAsync(condition, "CreatedTime Desc", qry.PageNo, qry.PageSize);
            var amount = await _memberRepository.GetAmount(condition);

            return Ok(ResponseUtility.CreateSuccessResopnse(members.Adapt<IEnumerable<MemberRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("certificate")]
        [SwaggerResponse(typeof(IEnumerable<MemberCertificateRM>))]
        public async ValueTask<IActionResult> GetCertificate([FromQuery] GetCertificate qry)
        {
            var member = await _memberCache.GetOneAsync($"{CacheKeys.SingleMember}{qry.MemberId}", o => o.Id == qry.MemberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(member.Adapt<MemberCertificateRM>()));
        }

        [HttpGet("point")]
        [SwaggerResponse(typeof(IEnumerable<PointJournalRM>))]
        public async ValueTask<IActionResult> GetPointJournal([FromQuery] GetPointJournal qry)
        {
            var journals = await _pointJournalRepository.GetWithSortingAsync(o => o.MemberId == qry.MemberId, "CreatedTime Desc", qry.PageNo, qry.PageSize);
            var amount = await _pointJournalRepository.GetAmount(o => o.MemberId == qry.MemberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(journals.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<PointJournalRM>>()
                , qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("order")]
        [SwaggerResponse(typeof(IEnumerable<OrderJournalRM>))]
        public async ValueTask<IActionResult> GetOrderJournal([FromQuery] GetOrderJournal qry)
        {
            var journals = await _orderRepository.GetWithSortingAsync(o => o.MemberId == qry.MemberId && o.State, "CreatedTime Desc", qry.PageNo, qry.PageSize);
            var amount = await _orderRepository.GetAmount(o => o.MemberId == qry.MemberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(journals.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<OrderJournalRM>>()
                , qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("compensation")]
        [SwaggerResponse(typeof(IEnumerable<CompensationJournalRM>))]
        public async ValueTask<IActionResult> GetCompensationJournal([FromQuery] GetCompensationJournal qry)
        {
            Expression<Func<CompensationJournals, bool>> condition = o => true;
            List<Expression<Func<CompensationJournals, bool>>> lsExp = new List<Expression<Func<CompensationJournals, bool>>>();
            if (qry.Name != null)
                lsExp.Add(o => o.Member.Name.Contains(qry.Name));
            if (qry.StartTime != null || qry.EndTime != null)
                lsExp.Add(o => o.CreatedTime >= qry.StartTime && o.CreatedTime <= qry.EndTime);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var journals = await _compensationJournalRepository.GetWithSortingAsync(condition, "CreatedTime Desc", qry.PageNo, qry.PageSize);
            var amount = await _compensationJournalRepository.GetAmount(condition);

            return Ok(ResponseUtility.CreateSuccessResopnse(journals.Adapt<IEnumerable<CompensationJournalRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("favorite")]
        [SwaggerResponse(typeof(IEnumerable<ComicFavoriteRM>))]
        public async ValueTask<IActionResult> GetComicFavorite([FromQuery] GetComicFavorite qry)
        {

            var favorites = await _comicFavoriteRepository.GetAsync(o => o.MemberId == qry.MemberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(favorites.OrderByDescending(o => o.CreatedTime).Adapt<IEnumerable<ComicFavoriteRM>>()));
        }

        [HttpGet("history")]
        [SwaggerResponse(typeof(IEnumerable<ComicHistoryRM>))]
        public async ValueTask<IActionResult> GetComicHistory([FromQuery] GetComicHistory qry)
        {
            var histories = await _comicHistoryRepository.GetAsync(o => o.MemberId == qry.MemberId);
            return Ok(ResponseUtility.CreateSuccessResopnse(histories.OrderByDescending(o => o.ReadingTime).Adapt<IEnumerable<ComicHistoryRM>>()));
        }

        [HttpPost()]
        public async ValueTask<IActionResult> UpdateMember(UpdateMember cmd)
        {
            var member = await _memberCache.GetOneAsync($"{CacheKeys.SingleMember}{cmd.Id}", o => o.Id == cmd.Id);
            member.UpdateMember(cmd.State);
            await _memberRepository.UpdateAsync(member);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{cmd.Id}");
            return Ok();
        }

        [HttpPost("compensation")]
        public async ValueTask<IActionResult> CompensateMember(CompensateMember cmd)
        {
            var member = await _memberRepository.GetOneAsync(o => o.Name == cmd.Name);
            if (member == null)
                throw new Exception("no such member name.");
            var managerId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(o => o.Type.Equals("sid"))?.Value ?? "0");
            await _memberRepository.CompensateMember(member, cmd.Type, cmd.Value, managerId);
            await _memberCache.ClearAsync($"{CacheKeys.SingleMember}{member.Id}");
            return Ok();
        }
    }
}
