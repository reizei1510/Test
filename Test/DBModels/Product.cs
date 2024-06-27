using System.ComponentModel.DataAnnotations;

namespace Test.DBModels
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Count { get; set; }
        public decimal Price { get; set; }

        public List<OrderProduct> OrderProducts { get; set; } = new();
    }
}
