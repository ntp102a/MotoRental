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
using Microsoft.VisualBasic;
using MotoRental.Extension;
using MotoRental.Helpper;
using MotoRental.Models;
using PagedList.Core;

namespace MotoRental.Areas.Jobber.Controllers
{
    [Area("Jobber")]
    [Authorize(Roles = "2")]
    public class JobberVehiclesController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        public INotyfService _notyfService { get; }
        public JobberVehiclesController(Rental_motorbikeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Jobber/JobberVehicles
        public async Task<IActionResult> Index(int page = 1, int brand_id = 0)
        {
            var accountID = User.Identity.GetAccountID();

            if (int.TryParse(accountID, out var userId))
            {
                var pageSize = 10;
                var pageNumber = page;
                List<Vehicle> lsvehicles = new List<Vehicle>();
                ViewBag.CurrentBrandID = brand_id;

                if (brand_id != 0)
                {
                    lsvehicles = _context.Vehicles
                        .AsNoTracking()
                        .Include(v => v.Brand)
                        .Include(v => v.Displacement)
                        .Include(v => v.User)
                        .Where(v => v.UserId == userId)
                        .Where(v => v.BrandId == brand_id)
                        .ToList();
                }
                else
                {
                    lsvehicles = _context.Vehicles
                        .AsNoTracking()
                        .Include(v => v.Brand)
                        .Include(v => v.Displacement)
                        .Include(v => v.User)
                        .Where(v => v.UserId == userId)
                        .ToList();
                }

                PagedList<Vehicle> models = new PagedList<Vehicle>(lsvehicles.AsQueryable(), pageNumber, pageSize);

                ViewBag.CurrentPage = pageNumber;

                ViewData["Brands"] = new SelectList(_context.Brands, "BrandId", "BrandName", brand_id);
                return View(models);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Filtter(int BrandID)
        {
            var url = $"/Jobber/JobberVehicles?brand_id={BrandID}";
            if (BrandID == 0)
            {
                url = $"/Jobber/JobberVehicles";
            }

            return Json(new { status = "success", RedirectUrl = url });
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementName");
            return View();
        }

        // POST: Jobber/JobberVehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle, Image image, IFormFile fThumb, IFormFile fimage1, IFormFile fimage2, IFormFile fimage3)
        {
            var accountID = User.Identity.GetAccountID();

            if (int.TryParse(accountID, out var userId))
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

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                vehicle.ImageId = image.ImageId;
                vehicle.UserId = userId;
                vehicle.Status = 0;
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm mới thành công");
            }
            return RedirectToAction(nameof(Index));
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", vehicle.BrandId);
            ViewData["DisplacementId"] = new SelectList(_context.Displacements, "DisplacementId", "DisplacementName", vehicle.DisplacementId);
            return View(vehicle);
        }

        // POST: Jobber/JobberVehicles/Edit/5
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

            var accountID = User.Identity.GetAccountID();

            if (int.TryParse(accountID, out var userId))
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
                vehicle.UserId = userId;
                vehicle.Status = 0;
                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
                _notyfService.Success("Sửa thành công");
            }
            else
            {
                _notyfService.Error("Sửa không thành công");
                return RedirectToAction(nameof(Edit));
            }
            return RedirectToAction(nameof(Index));
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
                return Problem("Không tìm thấy xe để xoá.");
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
