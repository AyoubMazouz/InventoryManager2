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

        public  IActionResult Index()
        {
            var items = _context.Items.ToList();
            return View(items);
        }

        public  IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        public  IActionResult Create()
        {
            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create(ItemViewModel model)
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

        public  IActionResult Edit(int id)
        {
            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();

            var model = new ItemViewModel
            {
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                CategoryId = item.CategoryId,
                SupplierId = item.SupplierId,
                ItemDetail = new ItemDetailViewModel
                {
                    Quantity = item.ItemDetail.Quantity,
                    Price = item.ItemDetail.Price,
                    Manufacturer = item.ItemDetail.Manufacturer,
                    Weight = item.ItemDetail.Weight,
                    Dimensions = item.ItemDetail.Dimensions,
                    Material = item.ItemDetail.Material,
                    Color = item.ItemDetail.Color,
                    CountryOfOrigin = item.ItemDetail.CountryOfOrigin,
                    ItemId = item.ItemDetail.ItemId,
                    ManufactureDate = item.ItemDetail.ManufactureDate,
                    ExpiryDate = item.ItemDetail.ExpiryDate,
                }
            };

            ViewBag.StatusList = this.GetStatusSelectList();
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(int id, ItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = this.GetStatusSelectList();
                ViewBag.Categories = new SelectList(_context.Category, "Id", "Name");
                ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "Name");

                return View(model);
            }

            var item = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include (i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();

            item.Name = model.Name;
            item.Description = model.Description;
            item.Status = model.Status;
            item.CategoryId = model.CategoryId;
            item.SupplierId = model.SupplierId;
            item.ItemDetail.Quantity = model.ItemDetail.Quantity;
            item.ItemDetail.Price = model.ItemDetail.Price;
            item.ItemDetail.Manufacturer = model.ItemDetail.Manufacturer;
            item.ItemDetail.Weight = model.ItemDetail.Weight;
            item.ItemDetail.Dimensions = model.ItemDetail.Dimensions;
            item.ItemDetail.Material = model.ItemDetail.Material;
            item.ItemDetail.Color = model.ItemDetail.Color;
            item.ItemDetail.CountryOfOrigin = model.ItemDetail.CountryOfOrigin;
            item.ItemDetail.ManufactureDate = (DateTime)model.ItemDetail.ManufactureDate;
            item.ItemDetail.ExpiryDate = (DateTime)model.ItemDetail.ExpiryDate;

            _context.Items.Update(item);
            _context.SaveChanges();

            this.Flash($"Item named {item.Name} has been Upadted Successfully!");

            return RedirectToAction(nameof(Index));
        }

        public  IActionResult Delete(int id)
        {
            var item = _context.Items
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  IActionResult DeleteConfirmed(int id)
        {
            var item = _context.Items
                .Include(i => i.ItemDetail)
                .FirstOrDefault(i => i.Id == id);

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
