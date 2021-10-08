using System.ComponentModel.DataAnnotations;

namespace Comic.BackOffice.Commands.Manager
{
    public class AddManager
    {
        [RegularExpression("^([a-z|A-Z]|\\d){4,10}$")]
        public string Name { get; set; }
        [RegularExpression("^\\S{4,10}$")]
        public string Password { get; set; }
    }
}
