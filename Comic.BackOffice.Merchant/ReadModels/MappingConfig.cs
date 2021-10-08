using Comic.Domain.Entities;
using Mapster;
using static Comic.BackOffice.Merchant.ReadModels.Report.PerformanceReportRM;

namespace Comic.BackOffice.Merchant.ReadModels
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Orders, Detail>()
                .Map(d => d.Id, s => s.MemberId)
                .Map(d => d.Name, s => s.Member.Name)
                .Map(d => d.ProductPrice, s => s.Product.Price);
        }
    }
}
