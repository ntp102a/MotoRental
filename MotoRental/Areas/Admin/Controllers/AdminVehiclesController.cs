using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoRental.Helpper;
using MotoRental.Models;
using PagedList.Core;

namespace MotoRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
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
                .Include(v => v.Image)
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

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Displacement)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", vehicle.BrandId);
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementName", vehicle.DisplacementId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", vehicle.UserId);
            return View(vehicle);
        }

        // POST: Admin/AdminVehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle, Image image, IFormFile fThumb, IFormFile fimage1, IFormFile fimage2, IFormFile fimage3)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    vehicle.VehicleName = Utilities.ToTitleCase(vehicle.VehicleName);

                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string images = Utilities.SEOUrl(vehicle.VehicleName) + extension;

                        image.ImageFont = await Utilities.UploadFile(fThumb, @"vehicles", images.ToLower());
                    }

                    if (fimage1 != null)
                    {
                        string extension = Path.GetExtension(fimage1.FileName);
                        string images = Utilities.SEOUrl(vehicle.VehicleName) + "_small1" + extension;

                        image.ImageLeftSide = await Utilities.UploadFile(fimage1, @"vehicles", images.ToLower());
                    }

                    if (fimage2 != null)
                    {
                        string extension = Path.GetExtension(fimage2.FileName);
                        string images = Utilities.SEOUrl(vehicle.VehicleName) + "_small2" + extension;

                        image.ImageRightSide = await Utilities.UploadFile(fimage2, @"vehicles", images.ToLower());
                    }

                    if (fimage3 != null)
                    {
                        string extension = Path.GetExtension(fimage3.FileName);
                        string images = Utilities.SEOUrl(vehicle.VehicleName) + "_small3" + extension;

                        image.ImageBackSide = await Utilities.UploadFile(fimage3, @"vehicles", images.ToLower());
                    }

                    if (string.IsNullOrEmpty(image.ImageFont)) image.ImageFont = "default.jpg";
                    if (string.IsNullOrEmpty(image.ImageLeftSide)) image.ImageLeftSide = "default.jpg";
                    if (string.IsNullOrEmpty(image.ImageRightSide)) image.ImageRightSide = "default.jpg";
                    if (string.IsNullOrEmpty(image.ImageBackSide)) image.ImageBackSide = "default.jpg";

                    _context.Images.Update(image);
                    await _context.SaveChangesAsync();

                    vehicle.ImageId = image.ImageId;
                    vehicle.Status = 0;
                    _context.Vehicles.Update(vehicle);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Sửa thành công");
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
                .Include(v => v.Image)
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

            var cart = _context.Carts.Include(x => x.Vehicle).Where(x => x.VehicleId == id).ToList();
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    _context.Carts.Remove(item);
                }
                _context.SaveChanges();
            }

            var orderdetail = _context.RentalDetails.Include(x => x.Vehicle).Where(x => x.VehicleId == id).ToList();
            if (orderdetail != null)
            {
                foreach (var item in orderdetail)
                {
                    _context.RentalDetails.Remove(item);
                }
                _context.SaveChanges();
            }

            var image = await _context.Images.FindAsync(vehicle?.ImageId);
            if (image != null)
            {
                _context.Images.Remove(image);
            }

            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xoá thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return (_context.Vehicles?.Any(e => e.VehicleId == id)).GetValueOrDefault();
        }
    }
}
