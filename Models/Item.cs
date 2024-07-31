using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le champ \"Nom\" est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le champ \"Nom\" doit avoir une longueur maximale de 100 caractères.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Le champ \"Description\" doit avoir une longueur maximale de 500 caractères.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Le champ \"Statut\" est obligatoire.")]
        public ItemStatus Status { get; set; }

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
