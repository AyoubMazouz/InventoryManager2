namespace InventoryManager2.Models
{
    public class ItemDetail
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Manufacturer { get; set; }
        public decimal? Weight { get; set; } // In kilograms
        public string? Dimensions { get; set; } // e.g. "10x10x10 cm"
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? CountryOfOrigin { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
    }
}
