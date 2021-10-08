using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;

namespace Comic.Repository
{
    public class VideoRepository : BaseRepository<Videos>, IVideoRepository
    {
        private readonly IDbContext _db;
        public VideoRepository(IDbContext db) : base(db)
        {
            _db = db;
        }

        public async ValueTask InsertVideo(Videos video)
        {
            _db.Session.BeginTransaction();
            try
            {
                await _db.InsertAsync<Videos>(video);
                var tagIds = new HashSet<int>();
                foreach (var item in video.Tags)
                {
                    var tag = _db.Query<VideoTags>(o => o.Name == item.Name).FirstOrDefault();
                    if (tag != null)
                    {
                        tagIds.Add(tag.Id);
                        continue;
                    }
                    var newTag = new VideoTags(item.Name);
                    var entity = await _db.InsertAsync(newTag);
                    tagIds.Add(Convert.ToInt32(entity.Id));
                }
                await _db.InsertRangeAsync(tagIds.Select(o => new VideoTagMapping(video.Cid, o)).ToList());
                var actorIds = new HashSet<int>();
                foreach (var item in video.Actors)
                {
                    var actor = _db.Query<VideoActors>(o => o.Name == item.Name).FirstOrDefault();
                    if (actor != null)
                    {
                        actorIds.Add(actor.Id);
                        continue;
                    }
                    var newActor = new VideoActors(item.Name);
                    var entity = await _db.InsertAsync(newActor);
                    actorIds.Add(Convert.ToInt32(entity.Id));
                }
                await _db.InsertRangeAsync(actorIds.Select(o => new VideoActorMapping(video.Cid, o)).ToList());
            }
            catch (Exception ex)
            {
                _db.Session.RollbackTransaction();
                throw new Exception($"InsertVideo {video.Cid} Error.");
            }
            _db.Session.CommitTransaction();
        }
    }
}
