using Web.Data;
using Web.Models;
using Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize]
    public class UserController : ViewController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10, string sortBy = "UserName", string sortDir = "asc")
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (page < 1) page = 1;
            if (pageSize < 10 || pageSize > 250) pageSize = 10;
            if (!string.IsNullOrEmpty(search))
                usersQuery = usersQuery
                    .Where(c => c.UserName.Contains(search) || c.Email.Contains(search));

            usersQuery = ApplySorting(usersQuery, sortBy, sortDir);

            var users = await usersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userVMs = new List<UserVM>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userVMs.Add(new UserVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    Roles = roles.ToList()
                });
            }

            var pagination = new PaginationVM<UserVM>
            {
                Items = userVMs,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = await usersQuery.CountAsync()
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
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var model = new UserVM
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.EmailConfirmed,
                PhoneNumberConfirmed = model.PhoneNumberConfirmed
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                this.Flash($"Un nouvel utilisateur nommé {model.UserName} a été créé !");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var model = new EditUserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserVM model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                this.Flash($"L'utilisateur nommé {user.UserName} a été mis à jour avec succès !");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                this.Flash($"L'utilisateur nommé {user.UserName} a été supprimé avec succès !");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(Index));
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortDir)
        {
            switch (sortBy.ToLower())
            {
                case "email":
                    return sortDir == "asc"
                        ? query.OrderBy(s => s.Email)
                        : query.OrderByDescending(s => s.Email);
                case "username":
                default:
                    return sortDir == "asc"
                        ? query.OrderBy(s => s.UserName)
                        : query.OrderByDescending(s => s.UserName);
            }
        }
    }
}
