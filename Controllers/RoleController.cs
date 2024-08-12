using InventoryManager2.Data;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager2.Controllers
{
    public class RoleController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Role> _roleManager;


        public RoleController(ApplicationDbContext context, RoleManager<Role> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public IActionResult Index(string search, int page = 1, int pageSize = 10, string sortBy = "UserName", string sortDir = "asc")
        {
            var roleQuery = _roleManager.Roles.AsQueryable();

            if (page < 1) page = 1;
            if (pageSize < 10 || pageSize > 250) pageSize = 10;
            if (!string.IsNullOrEmpty(search)) 
                roleQuery = roleQuery.Where(c => c.Name.Contains(search));

            roleQuery = ApplySorting(roleQuery, sortBy, sortDir);

            var roles = roleQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new RoleVM
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description
                })
            .ToList();

            var pagination = new PaginationVM<RoleVM>
            {
                Items = roles,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = roleQuery.Count()
            };

            ViewBag.search = search;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            ViewBag.sortBy = sortBy;
            ViewBag.sortDir = sortDir;

            return View(pagination);
        }

        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.Roles
                .Include(i => i.Users)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (role == null) return NotFound();

            var model = new RoleVM
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Users = role.Users
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUpdateRoleVM model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var role = new Role
            {
                Name = model.Name,
                Description = model.Description
            };

            await _roleManager.CreateAsync(role);

            this.Flash($"Un nouveau rôle nommé {model.Name} a été créé !");

            return RedirectToAction(nameof(Index));                 
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) return NotFound();

            var model = new CreateUpdateRoleVM
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CreateUpdateRoleVM model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid) return View(model);

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) return NotFound();

            role.Name = model.Name;
            role.Description = model.Description;

            await _roleManager.UpdateAsync(role);

            this.Flash($"Le rôle nommé {role.Name} a été mis à jour avec succès !");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) return NotFound();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                this.Flash($"Le rôle nommé {role.Name} a été supprimé avec succès !");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(Index));
        }

        
        private IQueryable<Role> ApplySorting(IQueryable<Role> query, string sortBy, string sortDir)
        {
            switch (sortBy.ToLower())
            {
                case "name":
                default:
                    return sortDir == "asc"
                        ? query.OrderBy(s => s.Name)
                        : query.OrderByDescending(s => s.Name);
            }
        }
    }

}
