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
    public class AdminRentalsController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        public INotyfService _notyfService { get; }

        public AdminRentalsController(Rental_motorbikeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminRentals
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var IsRentals = _context.Rentals
                .Include(x => x.User)
                .Include(x => x.Status)
                .AsNoTracking()
                .OrderByDescending(x => x.DateFrom);
            PagedList<Rental> models = new PagedList<Rental>(IsRentals, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminRentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rentals == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null || _context.Rentals == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
            .Include(x => x.User)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["TrangThai"] = new SelectList(_context.Statuses, "StatusId", "StatusName", rental.StatusId);
            return PartialView("ChangeStatus", rental);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("RentalId,DateFrom,DateTo,DateShip,Message,Price,StatusId,UserId")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.Rentals
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Status)
                    .FirstOrDefaultAsync(x => x.RentalId == id);
                    if (donhang != null)
                    {
                        rental.RentalId = donhang.RentalId;
                        rental.DateFrom = donhang.DateFrom;
                        rental.DateTo = donhang.DateTo;
                        rental.DateShip = donhang.DateShip;
                        rental.Message = donhang.Message;
                        rental.Price = donhang.Price;
                        rental.UserId = donhang.UserId;
                    }
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", rental.UserId);
            ViewData["TrangThai"] = new SelectList(_context.Statuses, "StatusId", "StatusName", rental.StatusId);
            return PartialView("ChangStatus", rental);
        }

        // GET: Admin/AdminRentals/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName");
            ViewData["TrangThai"] = new SelectList(_context.Statuses, "StatusId", "StatusName");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleName");
            return View();
        }

        // POST: Admin/AdminRentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,DateFrom,DateTo,DateShip,Message,Price,StatusId,UserId")] Rental rental, RentalDetail rentalDetail)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(rental);
                //await _context.SaveChangesAsync();
                //rentalDetail = new RentalDetail();
                //rentalDetail.RentalDetailId = rental.RentalId;
                //rentalDetail.VehicleId = 
                //rentalDetail.Amount = item.amount;
                //rentalDetail.TotalMoney = donhang.TotalMoney;
                //rentalDetail.Price = item.product.Price;
                //rentalDetail.CreateDate = DateTime.Now;
                //_context.Add(rentalDetail);
                //_notyfService.Success("Cập nhật thành công");
                //return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", rental.UserId);
            ViewData["TrangThai"] = new SelectList(_context.Statuses, "StatusId", "StatusName", rental.StatusId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleName", rentalDetail.VehicleId);
            return View(rental);
        }

        // GET: Admin/AdminRentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rentals == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", rental.UserId);
            ViewData["TrangThai"] = new SelectList(_context.Statuses, "StatusId", "StatusName", rental.StatusId);
            return View(rental);
        }

        // POST: Admin/AdminRentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,DateFrom,DateTo,DateShip,Message,Price,StatusId,UserId")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", rental.UserId);
            ViewData["TrangThai"] = new SelectList(_context.Statuses, "StatusId", "StatusName", rental.StatusId);
            return View(rental);
        }

        // GET: Admin/AdminRentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rentals == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Status)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Admin/AdminRentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rentals == null)
            {
                return Problem("Entity set 'Rental_motorbikeContext.Rentals'  is null.");
            }
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
          return (_context.Rentals?.Any(e => e.RentalId == id)).GetValueOrDefault();
        }
    }
}
