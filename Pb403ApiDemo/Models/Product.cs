namespace Pb403ApiDemo.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<ProductTag> ProductTags { get; set; } = [];
    }
}
