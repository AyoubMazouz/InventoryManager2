using InventoryManager2.Models;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.ViewModels
{
    public class ItemVM
    {
        [Display(Name = "Identifiant")]
        public int Id { get; set; }

        [Display(Name = "Titre")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Statut")]
        public Item.ItemStatus Status { get; set; }

        [Display(Name = "Catégorie")]
        public int? CategoryId { get; set; }

        [Display(Name = "Fournisseur")]
        public int? SupplierId { get; set; }

        [Display(Name = "Créé le")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Mis à jour le")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ItemDetailVM ItemDetail { get; set; }
        public List<CreateUpdateCustomFieldVM>? CustomFields { get; set; }
    }

    public class CreateUpdateItemVM
    {
        [Display(Name = "Titre")]
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string? Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "La description est obligatoire.")]
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }

        [Display(Name = "statut")]
        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public Item.ItemStatus Status { get; set; }

        [Display(Name = "Catégorie")]
        public int? CategoryId { get; set; }

        [Display(Name = "Fournisseur")]
        public int? SupplierId { get; set; }

        public ItemDetailVM ItemDetail { get; set; }
        public List<CreateUpdateCustomFieldVM>? CustomFields { get; set; }
    }

    public class ItemDetailVM
    {
        [Display(Name = "Quantité")]
        public int? Quantity { get; set; }

        [Display(Name = "Prix")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être un nombre positif.")]
        public decimal? Price { get; set; }

        [Display(Name = "Fabricant")]
        [StringLength(100, ErrorMessage = "Le nom du fabricant ne peut pas dépasser 100 caractères.")]
        public string? Manufacturer { get; set; }

        [Display(Name = "Poids (kg)")]
        [Range(0, double.MaxValue, ErrorMessage = "Le poids doit être un nombre positif.")]
        public decimal? Weight { get; set; } // En kilogrammes

        [Display(Name = "Dimensions")]
        [StringLength(50, ErrorMessage = "Les dimensions ne peuvent pas dépasser 50 caractères.")]
        public string? Dimensions { get; set; } // ex. "10x10x10 cm"

        [Display(Name = "Matériau")]
        [StringLength(50, ErrorMessage = "Le matériau ne peut pas dépasser 50 caractères.")]
        public string? Material { get; set; }

        [Display(Name = "Couleur")]
        [StringLength(50, ErrorMessage = "La couleur ne peut pas dépasser 50 caractères.")]
        public string? Color { get; set; }

        [Required(ErrorMessage = "La date de fabrication est obligatoire.")]
        [Display(Name = "Date de fabrication")]
        public DateTime ManufactureDate { get; set; }

        [Required(ErrorMessage = "La date d'expiration est obligatoire.")]
        [Display(Name = "Date d'expiration")]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Pays d'origine")]
        [StringLength(50, ErrorMessage = "Le pays d'origine ne peut pas dépasser 50 caractères.")]
        public string? CountryOfOrigin { get; set; }

        [Required(ErrorMessage = "L'ID de l'article est obligatoire.")]
        [Display(Name = "ID de l'article")]
        public int ItemId { get; set; }
    }
}
