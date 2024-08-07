using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public ItemStatus Status { get; set; }

        public List<CustomField>? CustomFields { get; set; }

        [Required]
        public int ItemDetailId { get; set; }

        [Required]
        public ItemDetail ItemDetail { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public enum ItemStatus
        {
            Available,
            OutOfStock,
            Discontinued,
            PendingRestock
        }
    }
}
