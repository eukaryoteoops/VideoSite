using System.ComponentModel.DataAnnotations;

namespace Comic.BackOffice.Merchant.Commands.Merchant
{
    public class UpdateMerchantPassword
    {
        [RegularExpression("^\\S{4,10}$")]
        public string OldPassword { get; set; }
        [RegularExpression("^\\S{4,10}$")]
        public string NewPassword { get; set; }
    }
}
