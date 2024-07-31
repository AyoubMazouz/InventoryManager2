using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{
    public class ItemDetail
    {
        public int Id { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Le champ \"Quantité\" doit être un nombre positif.")]
        public int? Quantity { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Le champ \"Prix\" doit être un nombre positif.")]
        public decimal? Price { get; set; }

        [StringLength(100, ErrorMessage = "Le champ \"Fabricant\" doit avoir une longueur maximale de 100 caractères.")]
        public string? Manufacturer { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Le champ \"Poids\" doit être un nombre positif.")]
        public decimal? Weight { get; set; } // In kilograms

        [StringLength(50, ErrorMessage = "Le champ \"Dimensions\" doit avoir une longueur maximale de 50 caractères.")]
        public string? Dimensions { get; set; } // e.g. "10x10x10 cm"

        [StringLength(50, ErrorMessage = "Le champ \"Matériau\" doit avoir une longueur maximale de 50 caractères.")]
        public string? Material { get; set; }

        [StringLength(50, ErrorMessage = "Le champ \"Couleur\" doit avoir une longueur maximale de 50 caractères.")]
        public string? Color { get; set; }

        [StringLength(50, ErrorMessage = "Le champ \"Pays d'origine\" doit avoir une longueur maximale de 50 caractères.")]
        public string? CountryOfOrigin { get; set; }

        [Required(ErrorMessage = "Le champ \"Date de fabrication\" est obligatoire.")]
        public DateTime ManufactureDate { get; set; }

        [Required(ErrorMessage = "Le champ \"Date d'expiration\" est obligatoire.")]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Le champ \"ID de l'article\" est obligatoire.")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Le champ \"Article\" est obligatoire.")]
        public Item Item { get; set; }
    }
}
