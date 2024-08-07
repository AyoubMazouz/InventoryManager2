using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{
    public class CustomField
    {
        public int Id { get; set; }
        
        [Required]
        public int ItemId { get; set; }

        [Required]
        public string DataType { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string? Value { get; set; }
        
        public Item Item { get; set; }
    } 
}
