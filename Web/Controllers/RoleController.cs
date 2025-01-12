using Web.Data;
using Web.Models;
using Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace Web.Controllers
{
    [Authorize]
    public class RoleController : ViewController
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Role> _roleManager;

        public RoleController(ApplicationDbContext context, RoleManager<Role> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        private readonly List<string> headers = new List<string>
        {
            "Identiant", "Nom", "Description", "Date de création", "Date de mise à jour"
        };

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

        public IActionResult Export(string exportType)
        {
            var roles = _roleManager.Roles.ToList();

            if (roles.Count == 0) return NotFound();

            switch (exportType.ToLower())
            {
                case "xlsx":
                    var excelFile = this.ExportToExcel(roles);
                    this.Flash($"La table des fournisseurs a été exportée avec succès !");
                    return excelFile;
                case "csv":
                    var csvFile = this.ExportToExcel(roles);
                    this.Flash($"La table des fournisseurs a été exportée avec succès !");
                    return csvFile;
                default:
                    return BadRequest("Invalid export type.");
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Import(ICollection<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ModelState.AddModelError("ExcelFile", "Please upload at least one Excel file.");
                return RedirectToAction(nameof(Index));
            }

            var roles = new List<Role>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;

                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];
                            var rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var role = new Role
                                {
                                    Name = worksheet.Cells[row, 2].Text,
                                    Description = worksheet.Cells[row, 3].Text,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now
                                };
                                roles.Add(role);
                            }
                        }
                    }
                }
            }
            var errors = new List<string>();

            foreach (var role in roles)
            {
                var existingRole = await _roleManager.FindByNameAsync(role.Name);
                if (existingRole == null)
                {
                    var result = await _roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        errors.AddRange(result.Errors.Select(e => e.Description));
                    }
                }
                else
                {
                    errors.Add($"Role '{role.Name}' already exists.");
                }
            }

            if (errors.Any())
            {
                this.Flash(string.Join("\n", errors), AlertType.Danger);
            }
            else
            {
                this.Flash("Roles imported successfully.", AlertType.Success);
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult ExportToExcel(List<Role> roles)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Suppliers");

                for (int i = 0; i < this.headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = this.headers[i];
                }

                int row = 2;
                foreach (var role in roles)
                {
                    worksheet.Cells[row, 1].Value = role.Id;
                    worksheet.Cells[row, 2].Value = role.Name;
                    worksheet.Cells[row, 3].Value = role.Description;
                    worksheet.Cells[row, 4].Value = role.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 5].Value = role.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"Roles-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, contentType, fileName);
            }
        }

        public IActionResult ExportToCsv(List<Role> roles)
        {
            var csv = new StringBuilder();

            csv.AppendLine(string.Join(",", this.headers));

            foreach (var role in roles)
            {
                var row = new List<string>
                {
                    role.Id.ToString(),
                    role.Name ?? "",
                    role.Description ?? "",
                    role.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    role.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };

                csv.AppendLine(string.Join(",", row));
            }

            var fileName = $"Roles-{DateTime.Now:yyyyMMddHHmmss}.csv";
            var contentType = "text/csv";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(bytes, contentType, fileName);
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
