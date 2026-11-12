namespace OnlineStoreBackend.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }
        public string? SKU { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal? Discount { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public decimal? Rating { get; set; }
        public string? Brand { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string? LastUpdatedBy { get; set; }
        public Inventory? Inventory { get; set; }
    }
}
