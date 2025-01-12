using Web.Models;

namespace Web.ViewModels
{
    public class RoleVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<User>? Users { get; set; }
    }

    public class CreateUpdateRoleVM
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
