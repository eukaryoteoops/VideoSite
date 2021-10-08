using System.Collections.Generic;
using System.Linq;
using Comic.Api.ReadModels.Advert;
using Comic.Api.ReadModels.Member;
using Comic.Api.ReadModels.Shelf;
using Comic.Domain.Entities;
using Mapster;

namespace Comic.Api.ReadModels
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<ComicHistories, ComicHistoryRM>()
                .Map(d => d.ComicTitle, s => s.Comic.Title);
            config.ForType<VideoHistories, VideoHistoryRM>()
                .Map(d => d.Name, s => s.Video.Name);
            config.ForType<ComicFavorites, ComicFavoriteRM>()
                .Map(d => d.ComicTitle, s => s.Comic.Title)
                .Map(d => d.Chapter, s => s.Comic.ChapterCount)
                .Map(d => d.UpdatedTime, s => s.Comic.UpdatedTime);
            config.ForType<VideoFavorites, VideoFavoriteRM>()
                .Map(d => d.Name, s => s.Video.Name)
                .Map(d => d.Cid, s => s.Video.Cid);
            config.ForType<IEnumerable<Adverts>, AdvertRM>()
                .Map(d => d.Comic, s => s.Where(o => o.Type == 1).OrderBy(o => o.Order))
                .Map(d => d.Member, s => s.Where(o => o.Type == 2).OrderBy(o => o.Order))
                .Map(d => d.Carousel, s => s.Where(o => o.Type == 3).OrderBy(o => o.Order))
                .Map(d => d.WebCover, s => s.Where(o => o.Type == 4).OrderBy(o => o.Order))
                .Map(d => d.AppCover, s => s.Where(o => o.Type == 5).OrderBy(o => o.Order))
                .Map(d => d.PopUp, s => s.Where(o => o.Type == 6).OrderBy(o => o.Order));
            config.ForType<Orders, MemberOrderRM>()
                .Map(d => d.Product, s => s.Product.Name)
                .Map(d => d.Payment, s => s.Payment.ChannelName)
                .Map(d => d.Price, s => s.Product.Price);
            config.ForType<PointJournals, MemberPointJournalRM>()
                .Map(d => d.Action, s => 1, cond => cond.Remark.Contains("-"))
                .Map(d => d.Action, s => 2, cond => cond.Remark == "系統補單")
                .Map(d => d.Action, s => 3, cond => cond.Remark == "點數方案");


        }
    }
}
