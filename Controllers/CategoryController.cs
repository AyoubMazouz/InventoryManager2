using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManager2.Data;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;

namespace InventoryManager2.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = await _context.Category
                .Include(c => c.Parent)
                .ToListAsync();

            return View(categories);
        }

        public IActionResult Details(int id)
        {
            var category = _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefault(m => m.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        public IActionResult Create()
        {
            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {

                ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", model.ParentId);
                return View(model);
            }

            var parent = await _context.Category.FindAsync(model.ParentId); 

            var category = new Category
            {
                Name = model.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                /*ParentId = model.ParentId,*/
            };
            if (parent != null) category.Parent = parent;

            _context.Add(category);
            await _context.SaveChangesAsync();

            this.Flash($"New category named {category.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefault(m => m.Id == id);

            if (category == null) return NotFound();

            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", category.ParentId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)

        {
            if (id == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Parent"] = new SelectList(
                    await _context.Category.ToListAsync(),
                    "Id", "Name", model.ParentId);
                return View(model);
            }

            var category = await _context.Category.FindAsync(id);

            if (category == null) return NotFound();

            var parent = await _context.Category.FindAsync(model.ParentId);

            category.Name = model.Name;
            category.UpdatedAt = DateTime.Now;
            category.ParentId = model.ParentId;
            if (parent != null) category.Parent = parent;

            _context.Update(category);
            await _context.SaveChangesAsync();

            this.Flash($"category named {model.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefault(m => m.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {
            if (id == null) return NotFound();

            var category = await _context.Category.FindAsync(id);

            if (category == null) return NotFound();

            if (this.HasDependencies(category))
            {
                this.Flash(
                    $"category named {category.Name} has one or more Subcategory that depends on it!", 
                    AlertType.Warn
                );
                return RedirectToAction(nameof(Delete));
            }

            // Check if Category can be deleted

            _context.Category.Remove(category);
            _context.SaveChanges();

            this.Flash($"category named {category.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }

        private bool HasDependencies(Category category)
        {
            return _context.Category.Any(p => p.ParentId == category.Id);
        }
    }
}
