using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RabbitMQExample.Models;
using RabbitMQExample.Services;

namespace RabbitMQExample.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductsController(AppDbContext context, RabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Products'  is null.");
        }


        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products products, IFormFile? ProductImage)
        {
            if (!ModelState.IsValid) return View(products);

            if (ProductImage is { Length: > 0 })
            {
                var imageName = Guid.NewGuid() + Path.GetExtension(ProductImage.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageName);
                await using FileStream fileStream = new(path, FileMode.Create);
                await ProductImage.CopyToAsync(fileStream);
                products.ProductImage = imageName;

            }
            else
            {
                var imageName = Guid.NewGuid() + Path.GetExtension("sample_picture.jpg");
                products.ProductImage = imageName;
                _rabbitMQPublisher.Publish(new NoneImage() { ImageId = products.ProductId.ToString(),ImageName= imageName });
            }
            _context.Add(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDbContext.Products'  is null.");
            }
            var products = await _context.Products.FindAsync(id);
            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
