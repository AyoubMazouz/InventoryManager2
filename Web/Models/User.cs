using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class User : IdentityUser
    {
        public ICollection<Item>? Items { get; set; }
        public ICollection<Role>? Roles { get; set; }
    }
}
