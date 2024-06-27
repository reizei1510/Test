using System.ComponentModel.DataAnnotations;

namespace Test.DBModels
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public List<OrderProduct> OrderProducts { get; set; } = new();
    }
}
