namespace Test.Dto
{
    public class OrderDto
    {
        public int? Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<string> ProductsList { get; set; }
        public List<decimal> CountsList { get; set; }
    }
}
