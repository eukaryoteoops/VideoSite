using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class VideoActorMapping : Entity
    {
        public VideoActorMapping()
        {
        }

        public VideoActorMapping(string videoId, int actorId)
        {
            VideoId = videoId;
            ActorId = actorId;
        }

        public string VideoId { get; set; }
        public int ActorId { get; set; }
        [Navigation("ActorId")]
        public VideoActors Actor { get; set; }


    }
}
