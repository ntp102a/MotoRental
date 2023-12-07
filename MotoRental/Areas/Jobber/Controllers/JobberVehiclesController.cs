using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoRental.Models;

namespace MotoRental.Areas.Jobber.Controllers
{
    [Area("Jobber")]
    [Authorize(Roles = "2")]
    public class JobberVehiclesController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public JobberVehiclesController(Rental_motorbikeContext context)
        {
            _context = context;
        }

        // GET: Jobber/JobberVehicles
        public async Task<IActionResult> Index()
        {
            var rental_motorbikeContext = _context.Vehicles.Include(v => v.Brand).Include(v => v.Displacement).Include(v => v.Image).Include(v => v.User);
            return View(await rental_motorbikeContext.ToListAsync());
        }

        // GET: Jobber/JobberVehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Displacement)
                .Include(v => v.Image)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Jobber/JobberVehicles/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId");
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementId");
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Jobber/JobberVehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,VehicleName,LicensePlate,Overview,PricePerDay,Kilometers,Status,RegDate,UpdationDate,UserId,BrandId,DisplacementId,ImageId")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", vehicle.BrandId);
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementId", vehicle.DisplacementId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", vehicle.ImageId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", vehicle.UserId);
            return View(vehicle);
        }

        // GET: Jobber/JobberVehicles/Edit/5
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
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", vehicle.ImageId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", vehicle.UserId);
            return View(vehicle);
        }

        // POST: Jobber/JobberVehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,VehicleName,LicensePlate,Overview,PricePerDay,Kilometers,Status,RegDate,UpdationDate,UserId,BrandId,DisplacementId,ImageId")] Vehicle vehicle)
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
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", vehicle.ImageId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", vehicle.UserId);
            return View(vehicle);
        }

        // GET: Jobber/JobberVehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Displacement)
                .Include(v => v.Image)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Jobber/JobberVehicles/Delete/5
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
