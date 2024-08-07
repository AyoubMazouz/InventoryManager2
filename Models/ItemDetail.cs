using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{
    public class ItemDetail
    {
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        [Range(0.2, double.MaxValue)]
        public decimal? Price { get; set; }

        [StringLength(255)]
        public string? Manufacturer { get; set; }

        [Range(0.2, double.MaxValue)]
        public decimal? Weight { get; set; } // In kilograms

        [StringLength(100)]
        public string? Dimensions { get; set; } // e.g. "10x10x10 cm"

        [StringLength(100)]
        public string? Material { get; set; }

        [StringLength(100)]
        public string? Color { get; set; }

        [StringLength(100)]
        public string? CountryOfOrigin { get; set; }

        public DateTime ManufactureDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int ItemId { get; set; }

        public Item Item { get; set; }
    }
}
