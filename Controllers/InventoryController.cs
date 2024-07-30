using InventoryManager2.Data;
using InventoryManager2.Models;
using InventoryManager2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace InventoryManager2.Controllers
{
    [Authorize]
    public class InventoryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = _context.Items.ToList();
            return View(items);
        }

        public async Task<IActionResult> ItemDetails(int? id)
        {
            if (id == null) return NotFound();

            var item = _context.Items.Include(i => i.ItemDetail).FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = this.GetStatusSelectList();
                ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
                ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

                return View(model);
            }

            var item = new Item
            {
                Name = model.Name,
                Description = model.Description,
                Status = model.Status,
                CategoryId = model.CategoryId,
                SupplierId = model.SupplierId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ItemDetail = new ItemDetail
                {
                    Quantity = model.ItemDetail.Quantity,
                    Price = model.ItemDetail.Price,        
                    Manufacturer = model.ItemDetail.Manufacturer, 
                    Weight = model.ItemDetail.Weight,  
                    Dimensions = model.ItemDetail.Dimensions,
                    Material = model.ItemDetail.Material,
                    Color = model.ItemDetail.Color,
                    CountryOfOrigin = model.ItemDetail.CountryOfOrigin, 
                    ItemId = model.ItemDetail.ItemId,
                    ManufactureDate = (DateTime)model.ItemDetail.ManufactureDate,
                    ExpiryDate = (DateTime)model.ItemDetail.ExpiryDate,
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Items.Add(item);
            _context.SaveChanges();

            this.Flash($"New Item named {item.Name} has been created!");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = _context.Items
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();

            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                ViewBag.StatusList = this.GetStatusSelectList();
                ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
                ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

                return View(item);
            }
            _context.Items.Update(item);
            _context.SaveChanges();

            this.Flash($"Item named {item.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = _context.Items
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Item item)
        {
            if (item == null) return NotFound();

            if (item.ItemDetail != null)
                _context.ItemDetails.Remove(item.ItemDetail);

            _context.Items.Remove(item);
            _context.SaveChanges();

            this.Flash($"Item named {item.Name} has been Deleted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public SelectList GetStatusSelectList()
        {
            return new SelectList(
                Enum.GetValues(typeof(Item.ItemStatus))
                .Cast<Item.ItemStatus>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }
            ).ToList(), "Value", "Text");
        }
    }
}
