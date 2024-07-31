using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{ 
    public class Supplier
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Le champ \"Nom\" est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le champ \"Nom\" doit avoir une longueur maximale de 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le champ \"Informations de contact\" est obligatoire.")]
        [StringLength(200, ErrorMessage = "Le champ \"Informations de contact\" doit avoir une longueur maximale de 200 caractères.")]
        public string ContactInfo { get; set; }
        public ICollection<Item>? Items { get; set; }
    }
    
}
