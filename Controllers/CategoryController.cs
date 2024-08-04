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

        public IActionResult Index()
        {
            var categories = _context.Category
                .Include(c => c.Parent)
                .ToList();

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
            if (ModelState.IsValid)
            {
                _context.Add(category);
                _context.SaveChanges();

                this.Flash($"New Category named {category.Name} has been created!");

                return RedirectToAction(nameof(Index));
            }

            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", category.ParentId);
            return View(category);
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
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(category);
                _context.SaveChanges();

                this.Flash($"category named {category.Name} has been Upadted Successfully!");

                return RedirectToAction(nameof(Index));
            }

            ViewData["Parent"] = new SelectList(_context.Category, "Id", "Name", category.ParentId);
            return View(category);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
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
