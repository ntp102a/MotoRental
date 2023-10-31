using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoRental.Models;
using PagedList.Core;

namespace MotoRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminVehiclesController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        public INotyfService _notyfService { get; }

        public AdminVehiclesController(Rental_motorbikeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminVehicles        
        public IActionResult Index(int page = 1, int BrandID = 0)
        {
            var pageNumber = page;
            var pageSize = 5;

            List<Vehicle> IsVehicals = new List<Vehicle>();
            ViewBag.CurrentBrandID = BrandID;
            if (BrandID != 0)
            {
                IsVehicals = _context.Vehicles
                .AsNoTracking()
                .Where(x => x.Brand.BrandId == BrandID)
                .Include(x => x.Brand)
                .Include(x => x.User)
                .OrderByDescending(x => x.VehicleId).ToList();
            }
            else
            {
                IsVehicals = _context.Vehicles
                .AsNoTracking()
                .Include(x => x.Brand)
                .Include(x => x.User)
                .OrderByDescending(x => x.VehicleId).ToList();
            }


            PagedList<Vehicle> models = new PagedList<Vehicle>(IsVehicals.AsQueryable(), pageNumber, pageSize);


            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Brands, "BrandId", "BrandName", BrandID);
            return View(models);
        }

        public IActionResult Filtter(int BrandID = 0)
        {
            var url = $"/Admin/AdminVehicles?BrandID={BrandID}";
            if (BrandID == 0)
            {
                url = $"/Admin/AdminVehicles";
            }

            return Json(new { status = "success", RedirectUrl = url });
        }

        // GET: Admin/AdminVehicles/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Admin/AdminVehicles/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId");
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Admin/AdminVehicles/Create
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

        // GET: Admin/AdminVehicles/Edit/5
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

        // POST: Admin/AdminVehicles/Edit/5
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

        // GET: Admin/AdminVehicles/Delete/5
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

        // POST: Admin/AdminVehicles/Delete/5
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
