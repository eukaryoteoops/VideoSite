using System;
using System.Linq;
using System.Threading.Tasks;
using Comic.Common.ExtensionMethods;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Hangfire.Server;

namespace Comic.Schedule.Jobs
{
    public class UpdateFreeComics
    {
        private readonly IChapterRepository _chapterRepository;

        public UpdateFreeComics(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public async ValueTask Trigger(PerformContext ctx)
        {
            var free = await _chapterRepository.GetAsync(o => o.Comic.Channel == ComicChannelEnum.同人誌 && o.Point == 0);
            var startTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).AddDays(-7).Date.WithOffset(8).ToUnixTimeSeconds();
            var endTime = startTime + 86400;
            var change = await _chapterRepository.GetAsync(o => o.Comic.Channel == ComicChannelEnum.同人誌 && o.EnabledTime >= startTime && o.EnabledTime < endTime);
            await _chapterRepository.UpdateFreeComics(free.Select(o => o.Id).ToList(), change.Take(3).Select(o => o.Id).ToList());

        }
    }
}
