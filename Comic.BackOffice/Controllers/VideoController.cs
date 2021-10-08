using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Videos;
using Comic.BackOffice.QueryModels.Video;
using Comic.BackOffice.ReadModels.Video;
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
    public class VideoController : ControllerBase
    {
        private readonly IVideoChannelRepository _videoChannelRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IVideoActorMappingRepository _videoActorMappingRepository;
        private readonly IVideoTagMappingRepository _videoTagMappingRepository;

        public VideoController(IVideoChannelRepository videoChannelRepository, IVideoRepository videoRepository, IVideoTagMappingRepository videoTagMappingRepository, IVideoActorMappingRepository videoActorMappingRepository)
        {
            _videoChannelRepository = videoChannelRepository;
            _videoRepository = videoRepository;
            _videoTagMappingRepository = videoTagMappingRepository;
            _videoActorMappingRepository = videoActorMappingRepository;
        }

        [HttpGet()]
        [SwaggerResponse(typeof(IEnumerable<VideoRM>))]
        public async Task<IActionResult> GetAll([FromQuery] GetVideos qry)
        {
            Expression<Func<Videos, bool>> condition = o => true;
            List<Expression<Func<Videos, bool>>> lsExp = new List<Expression<Func<Videos, bool>>>();
            if (qry.Cid != null)
                lsExp.Add(o => o.Cid == qry.Cid);
            if (qry.Name != null)
                lsExp.Add(o => o.Name.Contains(qry.Name));
            if (qry.Channel != null)
                lsExp.Add(o => o.Channel == qry.Channel);
            if (qry.State != null)
                lsExp.Add(o => o.State == qry.State);
            if (qry.StartDate != null || qry.EndDate != null)
                lsExp.Add(o => o.EnabledDate >= qry.StartDate && o.EnabledDate <= qry.EndDate);
            foreach (var exp in lsExp)
                condition = condition.AndAlso(exp);
            var comics = await _videoRepository.GetWithSortingAsync(condition, "EnabledDate Desc, Cid Asc", qry.PageNo, qry.PageSize);
            var amount = await _videoRepository.GetAmount(condition);
            return Ok(ResponseUtility.CreateSuccessResopnse(comics.Adapt<IEnumerable<VideoRM>>(), qry.PageNo, qry.PageSize, amount));
        }

        [HttpGet("{cid}")]
        [SwaggerResponse(typeof(VideoDetailRM))]
        public async Task<IActionResult> GetVideo([FromRoute] string cid)
        {
            var comic = await _videoRepository.GetOneAsync(o => o.Cid == cid);
            var tags = await _videoTagMappingRepository.GetAsync(o => o.VideoId == cid);
            var actors = await _videoActorMappingRepository.GetAsync(o => o.VideoId == cid);
            var views = 20;
            var result = comic.Adapt<VideoDetailRM>();
            result.AddInfo(tags, actors, views);
            return Ok(ResponseUtility.CreateSuccessResopnse(result));
        }


        [HttpGet("channel")]
        [SwaggerResponse(typeof(IEnumerable<ChannelRM>))]
        public async Task<IActionResult> GetAllChannels()
        {
            var products = await _videoChannelRepository.GetAsync();
            return Ok(ResponseUtility.CreateSuccessResopnse(products.Adapt<IEnumerable<ChannelRM>>()));
        }


        [HttpPatch("point")]
        public async Task<IActionResult> UpdatePoint(UpdateVideoPoint cmd)
        {
            var video = await _videoRepository.GetOneAsync(o => o.Cid == cmd.Cid);
            video.UpdatePoint(cmd.Point);
            await _videoRepository.UpdateAsync(video);
            return Ok();
        }

        [HttpPatch("state")]
        public async Task<IActionResult> UpdateState(UpdateVideoState cmd)
        {
            var video = await _videoRepository.GetOneAsync(o => o.Cid == cmd.Cid);
            video.UpdateState(cmd.State);
            await _videoRepository.UpdateAsync(video);
            return Ok();
        }


        [HttpPatch("channel/state")]
        public async Task<IActionResult> UpdateChannelState(UpdateChannelState cmd)
        {
            var channel = await _videoChannelRepository.GetOneAsync(o => o.Id == cmd.Id);
            channel.UpdateState(cmd.State);
            await _videoChannelRepository.UpdateAsync(channel);
            return Ok();
        }

        [HttpPatch("channel/order")]
        public async Task<IActionResult> UpdateChannelOrder(UpdateChannelOrder cmd)
        {
            await _videoChannelRepository.UpdateChannelOrder(cmd.ChannelIds);
            return Ok();
        }
    }
}
