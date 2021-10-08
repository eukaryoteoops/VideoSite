using System;
using Comic.BackOffice.ReadModels.Comic;
using Comic.BackOffice.ReadModels.Member;
using Comic.BackOffice.ReadModels.Order;
using Comic.BackOffice.ReadModels.Video;
using Comic.Domain.Entities;
using Mapster;
using static Comic.BackOffice.ReadModels.Merchant.MerchantReportRM;

namespace Comic.BackOffice.ReadModel
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Members, MemberRM>()
                .Map(d => d.IsVip, s => s.PackageTime > DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            config.ForType<CompensationJournals, CompensationJournalRM>()
                .Map(d => d.Name, s => s.Member.Name)
                .Map(d => d.ManagerName, s => s.Manager.Name);

            config.ForType<Orders, OrderRM>()
                .Map(d => d.MemberName, s => s.Member.Name)
                .Map(d => d.PaymentName, s => s.Payment.Name)
                .Map(d => d.ChannelName, s => s.Payment.ChannelName)
                .Map(d => d.ProductName, s => s.Product.Name)
                .Map(d => d.ProductPrice, s => s.Product.Price);

            config.ForType<Orders, OrderJournalRM>()
                .Map(d => d.PaymentName, s => s.Payment.Name)
                .Map(d => d.ChannelName, s => s.Payment.ChannelName)
                .Map(d => d.ProductName, s => s.Product.Name)
                .Map(d => d.ProductPrice, s => s.Product.Price);

            config.ForType<Orders, Detail>()
                .Map(d => d.Id, s => s.MemberId)
                .Map(d => d.Name, s => s.Member.Name)
                .Map(d => d.ProductName, s => s.Product.Name)
                .Map(d => d.ProductPrice, s => s.Product.Price);

            config.ForType<Videos, VideoRM>()
                .Map(d => d.Channel, s => (byte)s.Channel);
            config.ForType<Comics, ComicRM>()
                .Map(d => d.Channel, s => (byte)s.Channel);

            config.ForType<ComicFavorites, ComicFavoriteRM>()
                .Map(d => d.ComicTitle, s => s.Comic.Title)
                .Map(d => d.Chapter, s => s.Comic.ChapterCount)
                .Map(d => d.UpdatedTime, s => s.CreatedTime);
        }
    }
}