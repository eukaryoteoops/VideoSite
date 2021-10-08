using System.Linq;
using System.Threading.Tasks;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Hangfire.Server;

namespace Comic.Schedule.Jobs
{
    public class VideoStatisticJob
    {
        private readonly IVideoCounterRepository _videoCounterRepository;
        private readonly IVideoFavoriteRepository _videoFavoriteRepository;
        private readonly IVideoStatisticRepository _videoStatisticRepository;

        public VideoStatisticJob(IVideoCounterRepository videoCounterRepository, IVideoFavoriteRepository videoFavoriteRepository, IVideoStatisticRepository videoStatisticRepository)
        {
            _videoCounterRepository = videoCounterRepository;
            _videoFavoriteRepository = videoFavoriteRepository;
            _videoStatisticRepository = videoStatisticRepository;
        }

        /// <summary>
        ///     1 : 點擊
        ///     2 : 收藏
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async ValueTask Trigger(PerformContext ctx)
        {
            var allCounters = await _videoCounterRepository.GetAsync();
            var allFavorites = await _videoFavoriteRepository.GetAsync();
            await _videoStatisticRepository.UpdateStatistics(
                allCounters.GroupBy(o => o.Cid).Select(o => new VideoStatistics(1, o.Key, o.Count())).ToList(),
                allFavorites.GroupBy(o => o.Cid).Select(o => new VideoStatistics(2, o.Key, o.Count())).ToList());
        }
    }
}
