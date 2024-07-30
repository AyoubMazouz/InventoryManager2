using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManager2.Data;
using InventoryManager2.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InventoryManager2.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = _context.Category
                .Include(c => c.Parent)
                .ToList();

            return View(categories);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(m => m.Id == id);

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
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();

                this.Flash($"New Category named {category.Name} has been created!");

                return RedirectToAction(nameof(Index));
            }

            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", category.ParentId);
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) return NotFound();

            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", category.ParentId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();

                this.Flash($"category named {category.Name} has been Upadted Successfully!");

                return RedirectToAction(nameof(Index));
            }

            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", category.ParentId);
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Category
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Category category)
        {
            if (category == null) return NotFound();

            if (this.HasDependencies(category))
            {
                this.Flash(
                    $"category named {category.Name} has one or more Subcategory that depends on it!", 
                    AlertType.Warn
                );
                return RedirectToAction(nameof(Delete));
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

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
