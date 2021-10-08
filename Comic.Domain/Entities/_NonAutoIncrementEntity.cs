using Chloe.Annotations;

namespace Comic.Domain.Entities
{
    public class NonAutoIncrementEntity
    {
        [NonAutoIncrement]
        public int Id { get; set; }
    }
}
