using InventoryManager2.Models;

namespace InventoryManager2.ViewModels
{
    public class ItemViewModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Item.ItemStatus Status { get; set; }
        public int ItemDetiailId { get; set; }
        public ItemDetailViewModel ItemDetail { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }

    public class ItemDetailViewModel
    {
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Manufacturer { get; set; }
        public decimal? Weight { get; set; } // In kilograms
        public string? Dimensions { get; set; } // e.g. "10x10x10 cm"
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? CountryOfOrigin { get; set; }
        public int ItemId { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
