using System;
using System.Linq;
using System.Threading.Tasks;
using Comic.Common.ExtensionMethods;
using Comic.Domain.Repositories;
using Hangfire.Server;

namespace Comic.Schedule.Jobs
{
    public class UpdateDailyComics
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IComicRepository _comicRepository;

        public UpdateDailyComics(IChapterRepository chapterRepository, IComicRepository comicRepository)
        {
            _chapterRepository = chapterRepository;
            _comicRepository = comicRepository;
        }



        public async ValueTask Trigger(PerformContext ctx)
        {
            var startTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).Date.WithOffset(8).ToUnixTimeSeconds();
            var endTime = startTime + 86400;
            var chapters = await _chapterRepository.GetAsync(o => o.EnabledTime < endTime);
            await _comicRepository.UpdateDailyComics(chapters.OrderBy(o => o.Id));

        }
    }
}
