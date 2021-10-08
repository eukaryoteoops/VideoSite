using System.ComponentModel.DataAnnotations;

namespace Comic.BackOffice.Commands.Merchant
{
    public class AddMerchant
    {
        [RegularExpression("^([a-z|A-Z]|\\d){4,10}$")]
        public string Name { get; set; }
        public string NickName { get; set; }
        public int Bonus { get; set; }
    }
}
