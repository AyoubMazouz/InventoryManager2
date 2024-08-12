using InventoryManager2.Data;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManager2.Controllers
{
    public class UserController : Controller
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
            if (!string.IsNullOrEmpty(search)) usersQuery = usersQuery
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
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            return View();
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
