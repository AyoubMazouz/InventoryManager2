using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManager2.Data;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;

namespace InventoryManager2.Controllers
{
    public class SupplierController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public  IActionResult Index()
        {
            return View(_context.Supplier.ToList());
        }

        public  IActionResult Details(int? id)
        {

            if (id == null) return NotFound();

            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create(Supplier supplier)
        {
            if (!ModelState.IsValid) return View(model);

            var supplier = new Supplier
            {
                Name = model.Name,
                ContactInfo = model.ContactInfo,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            _context.Add(supplier);
            _context.SaveChanges();

            this.Flash($"New Category named {model.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public  IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(int id, Supplier supplier)
        {
            if (id == null) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(supplier);
                _context.SaveChanges();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            supplier.Name = model.Name;
            supplier.ContactInfo = model.ContactInfo;

            _context.Update(supplier);
            await _context.SaveChangesAsync();

            this.Flash($"Supplier named {supplier.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));

        }

        public  IActionResult Delete(int id)
        {
            var supplier = _context.Supplier.Find(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  IActionResult DeleteConfirmed(int id)
        {
            var supplier = _context.Supplier.FirstOrDefault(i => i.Id == id);

            if (supplier == null) return NotFound();

            // Check if there are no constrains

             _context.Supplier.Remove(supplier);
            _context.SaveChanges();

            this.Flash($"supplier named {supplier.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }
    }
}
