using System;
using System.Linq;
using System.Threading.Tasks;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Hangfire.Server;

namespace Comic.Schedule.Jobs
{
    public class ComicStatisticJob
    {
        private readonly IComicCounterRepository _comicCounterRepository;
        private readonly IComicFavoriteRepository _comicFavoriteRepository;
        private readonly IComicStatisticRepository _comicStatisticRepository;

        public ComicStatisticJob(IComicCounterRepository comicCounterRepository, IComicFavoriteRepository comicFavoriteRepository, IComicStatisticRepository comicStatisticRepository)
        {
            _comicCounterRepository = comicCounterRepository;
            _comicFavoriteRepository = comicFavoriteRepository;
            _comicStatisticRepository = comicStatisticRepository;
        }

        /// <summary>
        ///     1 : 點擊
        ///     2 : 收藏
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async ValueTask Trigger(PerformContext ctx)
        {
            var now = DateTimeOffset.UtcNow.AddMinutes(-10).ToOffset(TimeSpan.FromHours(8)).ToUnixTimeSeconds();
            var allCounters = await _comicCounterRepository.GetAsync(o => o.CreatedTime >= now);
            var allFavorites = await _comicFavoriteRepository.GetAsync(o => o.CreatedTime >= now);
            await _comicStatisticRepository.UpdateStatistics(
                allCounters.GroupBy(o => o.ComicId).Select(o => new ComicStatistics(1, o.Key, o.Count())).ToList(),
                allFavorites.GroupBy(o => o.ComicId).Select(o => new ComicStatistics(2, o.Key, o.Count())).ToList());
        }
    }
}
