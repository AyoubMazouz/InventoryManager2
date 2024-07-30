﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManager2.Data;
using InventoryManager2.Models;

namespace InventoryManager2.Controllers
{
    public class SupplierController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Supplier.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null) return NotFound();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid) return View(supplier);
            
            _context.Add(supplier);
            await _context.SaveChangesAsync();

            this.Flash($"New Category named {supplier.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(supplier);
                await _context.SaveChangesAsync();

                this.Flash($"Supplier named {supplier.Name} has been Upadted Successfully!");

                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Supplier.FindAsync(id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Supplier supplier)
        {
            if (supplier == null) return NotFound();

             _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();

            this.Flash($"supplier named {supplier.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }
    }
}
