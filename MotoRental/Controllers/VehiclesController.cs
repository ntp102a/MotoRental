using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoRental.Models;
using PagedList.Core;

namespace MotoRental.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public VehiclesController(Rental_motorbikeContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        [Route("shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var IsPages = _context.Vehicles
                    .AsNoTracking().Include(v => v.Brand)
                    .Include(v => v.Displacement)
                    .Include(v => v.User)
                    .OrderByDescending(x => x.UpdationDate);
                PagedList<Vehicle> models = new PagedList<Vehicle>(IsPages, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Vehicles/Details/5
        [Route("{id}.html", Name = "VehicleDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Vehicles
                    .Include(v => v.Brand)
                .Include(v => v.Displacement)
                .Include(v => v.User)
                    .FirstOrDefault(x => x.VehicleId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }

                var lsProduct = _context.Vehicles
                    .AsNoTracking()
                    .Include(v => v.Brand)
                .Include(v => v.Displacement)
                .Include(v => v.User)
                    .Where(x => x.BrandId == product.BrandId && x.VehicleId != id && x.Status == 0)
                    .OrderByDescending(x => x.UpdationDate)
                    .Take(3)
                    .ToList();
                ViewBag.Sanpham = lsProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId");
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,VehicleName,LicensePlate,Overview,Image,PricePerDay,Kilometers,Status,RegDate,UpdationDate,UserId,BrandId,DisplacementId")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", vehicle.BrandId);
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementId", vehicle.DisplacementId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", vehicle.UserId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", vehicle.BrandId);
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementId", vehicle.DisplacementId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", vehicle.UserId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,VehicleName,LicensePlate,Overview,Image,PricePerDay,Kilometers,Status,RegDate,UpdationDate,UserId,BrandId,DisplacementId")] Vehicle vehicle)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", vehicle.BrandId);
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementId", vehicle.DisplacementId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", vehicle.UserId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Displacement)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'Rental_motorbikeContext.Vehicles'  is null.");
            }
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return (_context.Vehicles?.Any(e => e.VehicleId == id)).GetValueOrDefault();
        }
    }
}
