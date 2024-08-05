using InventoryManager2.Models;

namespace InventoryManager2.ViewModels
{
    public class ItemVM
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Item.ItemStatus Status { get; set; }
        public ItemDetailVM ItemDetail { get; set; }
        public CategoryVM? Category { get; set; }
        public SupplierVM? Supplier { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateUpdateItemVM
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Item.ItemStatus Status { get; set; }
        public ItemDetailVM ItemDetail { get; set; }
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
    }

    public class ItemDetailVM
    {
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Manufacturer { get; set; }
        public decimal? Weight { get; set; } // In kilograms
        public string? Dimensions { get; set; } // e.g. "10x10x10 cm"
        public string? Material { get; set; }
        public string? Color { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? CountryOfOrigin { get; set; }
        public int ItemId { get; set; }
    }
    
}
