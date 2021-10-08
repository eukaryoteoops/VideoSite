using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Comic;
using Comic.BackOffice.QueryModels.Comic;
using Comic.BackOffice.ReadModels.Comic;
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
    public class ComicController : ControllerBase
    {
        private readonly IComicRepository _comicRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IComicTagMappingRepository _comicTagMappingRepository;

        public ComicController(IComicRepository comicRepository, IChapterRepository chapterRepository, ITagRepository tagRepository, IComicTagMappingRepository comicTagMappingRepository)
        {
            _comicRepository = comicRepository;
            _chapterRepository = chapterRepository;
            _tagRepository = tagRepository;
            _comicTagMappingRepository = comicTagMappingRepository;
        }

        [HttpGet()]
        [SwaggerResponse(typeof(IEnumerable<ComicRM>))]
        public async ValueTask<IActionResult> GetAll([FromQuery] GetComics qry)
        {
            Expression<Func<Comics, bool>> condition = o => true;
            List<Expression<Func<Comics, bool>>> lsExp = new List<Expression<Func<Comics, bool>>>();
            if (qry.Id != null)
                lsExp.Add(o => o.Id == qry.Id);
            if (qry.Channel != null)
                lsExp.Add(o => o.Channel == (ComicChannelEnum)qry.Channel);
            if (qry.Title != null)
                lsExp.Add(o => o.Title.Contains(qry.Title));
            if (qry.IsEnded != null)
                lsExp.Add(o => o.IsEnded == qry.IsEnded);
            if (qry.State != null)
                lsExp.Add(o => o.State == qry.State);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var comics = await _comicRepository.GetWithSortingAsync(condition, "Id Desc", qry.PageNo, qry.PageSize);
            var amount = await _comicRepository.GetAmount(condition);

            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<ComicRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("{comicId}")]
        [SwaggerResponse(typeof(ComicDetailRM))]
        public async ValueTask<IActionResult> GetDetail([FromRoute] int comicId)
        {
            var comic = await _comicRepository.GetOneAsync(o => o.Id == comicId);
            var tags = await _comicTagMappingRepository.GetAsync(o => o.ComicId == comicId);
            var result = comic.Adapt<ComicDetailRM>();
            result.Tags = tags.Select(o => o.Tag.Name).Distinct();
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        [HttpGet("chapter")]
        [SwaggerResponse(typeof(IEnumerable<ChapterRM>))]
        public async ValueTask<IActionResult> GetAllChapters([FromQuery] GetChapters qry)
        {
            var chapters = await _chapterRepository.GetWithSortingAsync(o => o.ComicId == qry.ComicId, "Number Asc", qry.PageNo, qry.PageSize);
            var amount = await _chapterRepository.GetAmount(o => o.ComicId == qry.ComicId);

            return Ok(ResponseUtility.CreateSuccessResopnse(chapters.Adapt<IEnumerable<ChapterRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("statistic")]
        [SwaggerResponse(typeof(IEnumerable<ComicStatisticRM>))]
        public async ValueTask<IActionResult> GetComicStatistic([FromQuery] GetComicStatistic qry)
        {
            ;
            var startTime = new DateTimeOffset(qry.Year, qry.Month, 1, 0, 0, 0, TimeSpan.FromHours(8)).ToUnixTimeSeconds();
            var endTime = new DateTimeOffset(qry.Year, qry.Month, 1, 0, 0, 0, TimeSpan.FromHours(8)).AddDays(DateTime.DaysInMonth(qry.Year, qry.Month)).ToUnixTimeSeconds();
            var chapters = await _chapterRepository.GetAsync(o => o.EnabledTime >= startTime && o.EnabledTime < endTime);
            var result = new List<ComicStatisticRM>();
            for (int i = 1; i <= DateTime.DaysInMonth(qry.Year, qry.Month); i++)
            {
                var date = new DateTimeOffset(qry.Year, qry.Month, i, 0, 0, 0, TimeSpan.FromHours(8));
                var count = chapters.Count(o => o.EnabledTime >= date.ToUnixTimeSeconds() && o.EnabledTime < date.AddDays(1).ToUnixTimeSeconds());
                result.Add(new ComicStatisticRM { Date = date, Count = count });
            }
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        [HttpGet("statistic/detail")]
        [SwaggerResponse(typeof(IEnumerable<ComicStatisticDetailRM>))]
        public async ValueTask<IActionResult> GetComicStatisticDetail([FromQuery] GetComicStatisticDetail qry)
        {
            var startTime = new DateTimeOffset(qry.Year, qry.Month, qry.Day, 0, 0, 0, TimeSpan.FromHours(8)).ToUnixTimeSeconds();
            var endTime = new DateTimeOffset(qry.Year, qry.Month, qry.Day, 0, 0, 0, TimeSpan.FromHours(8)).AddDays(1).ToUnixTimeSeconds();
            var chapters = await _chapterRepository.GetAsync(o => o.EnabledTime >= startTime && o.EnabledTime < endTime);
            var result = chapters.GroupBy(o => o.ComicId).Select(o => new ComicStatisticDetailRM(o.Key, o.ToList().First().Comic.Title, o.ToList().Count));
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }

        [HttpPatch("tags")]
        public async Task<IActionResult> AddOrUpdateTags(AddTags cmd)
        {
            var tagIds = new HashSet<int>();
            foreach (var i in cmd.Tags.Select(o => o.Trim()))
            {
                var tag = await _tagRepository.GetOneAsync(o => o.Name == i);
                if (tag != null)
                {
                    tagIds.Add(tag.Id);
                    continue;
                }

                var newTag = new ComicTags(i);
                var entity = await _tagRepository.AddAsync(newTag);
                tagIds.Add(Convert.ToInt32(entity.Id));
            }
            return Ok(ResponseUtility.CreateSuccessResopnse(new TagRM() { Ids = tagIds }));
        }

        [HttpPatch()]
        public async Task<IActionResult> AddComic(AddComic cmd)
        {
            var comic = new Comics(cmd.Title, cmd.Channel, cmd.Author, cmd.Desc, cmd.IsEnded);
            await _comicRepository.AddComicAsync(comic, cmd.Tags);
            return Ok();
        }

        [HttpPatch("chapter")]
        public async Task<IActionResult> AddChapter(AddChapter cmd)
        {
            var chapter = new Chapters(cmd.ComicId, cmd.Number, cmd.Title, cmd.Point, cmd.Count, cmd.EnabledTime);
            await _chapterRepository.AddChapter(chapter);
            return Ok();
        }

        [HttpPost("state")]
        public async ValueTask<IActionResult> UpdateComicState(UpdateComicState cmd)
        {
            var comic = await _comicRepository.GetOneAsync(o => o.Id == cmd.Id);
            comic.UpdateState(cmd.State);
            await _comicRepository.UpdateAsync(comic);
            return Ok();
        }

        [HttpPost()]
        public async ValueTask<IActionResult> UpdateComic(UpdateComic cmd)
        {
            var comic = await _comicRepository.GetOneAsync(o => o.Id == cmd.Id);
            comic.UpdateComic(cmd.Title, cmd.Channel, cmd.Author, cmd.Desc, cmd.IsEnded);
            await _comicRepository.UpdateComic(comic, cmd.Tags);
            return Ok();
        }

        [HttpPost("chapter")]
        public async ValueTask<IActionResult> UpdateChapter(UpdateChapter cmd)
        {
            var chapter = await _chapterRepository.GetOneAsync(o => o.Id == cmd.Id);
            chapter.UpdateChapter(cmd.Title, cmd.Point, cmd.Count, cmd.EnabledTime);
            await _chapterRepository.UpdateAsync(chapter);
            return Ok();
        }
    }
}
