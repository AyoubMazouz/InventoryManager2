using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManager2.Models
{
    public class User : IdentityUser    
    {
        public ICollection<Item>? Items { get; set; }
    }
}
