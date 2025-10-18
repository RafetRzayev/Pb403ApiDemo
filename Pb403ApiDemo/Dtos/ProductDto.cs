namespace Pb403ApiDemo.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }
        public List<string>? TagNames { get; set; }
    }

    public class CreateProductDto
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; } = [];
    }

    public class UpdateProductDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
