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
using MotoRental.Extension;
using MotoRental.Models;
using MotoRental.ModelViews;
using PagedList.Core;

namespace MotoRental.Areas.Jobber.Controllers
{
    [Area("Jobber")]
    [Authorize(Roles = "2")]
    public class JobberRentalsController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        public INotyfService _notyfService { get; }

        public JobberRentalsController(Rental_motorbikeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Jobber/JobberRentals
        public IActionResult Index(int? page)
        {
            var accountID = User.Identity.GetAccountID();

            List<Rental> lsOrders = new List<Rental>();

            if (int.TryParse(accountID, out var userId))
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 20;

                var orderDetails = _context.RentalDetails
                    .AsNoTracking()
                .Include(x => x.Vehicle)
                .Include(x => x.Rental)
                .Where(x => x.Vehicle.UserId == userId)
                .ToList();
                foreach (var item in orderDetails)
                {
                    var lsOrder = _context.Rentals
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Status)
                    .FirstOrDefault(x => x.RentalId == item.RentalId);
                    lsOrders.Add(lsOrder);
                }
                
                PagedList<Rental> models = new PagedList<Rental>(lsOrders.AsQueryable().OrderByDescending(x => x.RentalId), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            else
            {
                return View();
            }
        }

        // GET: Jobber/JobberRentals/Details/5
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
            var Chitietdonhang = _context.RentalDetails
                .AsNoTracking()
                .Include(x => x.Vehicle)
                .Include(x => x.Rental)
                .Where(x => x.RentalId == rental.RentalId)
                .OrderBy(x => x.RentalDetailId)
                .ToList();
            ViewBag.ChiTiet = Chitietdonhang;

            return View(rental);
        }

        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null || _context.Rentals == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(o => o.User)
                .Include(o => o.Status)
                .Include(o => o.RentalDetails)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            //.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["TrangThai"] = new SelectList(_context.Statuses, "StatusId", "StatusName", rental.StatusId);
            return PartialView("ChangeStatus", rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, Rental rental)
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
                        .FirstOrDefaultAsync(x => x.RentalId == id);
                    if (donhang != null)
                    {
                        donhang.StatusId = rental.StatusId;
                        donhang.Phone = donhang.User.Phone;
                    }
                    _context.Rentals.Update(donhang);
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
            return PartialView("ChangStatus", rental);
        }

        // GET: Jobber/JobberRentals/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName");
            return View();
        }

        // POST: Jobber/JobberRentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,DateFrom,DateTo,DateShip,Message,Price,Status,UserId")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rental);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", rental.UserId);
            return View(rental);
        }

        // GET: Jobber/JobberRentals/Edit/5
        public async Task<IActionResult> Edit(int? id, RentalViewModel model)
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
            else
            {
                model.RentalId = rental.RentalId;
                model.RentalName = rental.RentalName;
                model.Phone= rental.Phone;
                model.Email = rental.Email;
                model.Address= rental.Address;
                model.DateFrom= rental.DateFrom;
                model.DateTo= rental.DateTo;
                model.DateShip= rental.DateShip;
                model.StatusId = rental.StatusId;
                model.UserId= rental.UserId;
                model.Message= rental.Message;
                model.Price = rental.Price;
            }

            ViewData["Status"] = new SelectList(_context.Statuses, "StatusId", "StatusName", model.StatusId);
            return View(model);
        }

        // POST: Jobber/JobberRentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RentalViewModel model)
        {
            if (id != model.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var rental = _context.Rentals.SingleOrDefault(x => x.RentalId == id);
                    if (rental != null)
                    {
                        rental.RentalName = model.RentalName;
                        rental.Phone = model.Phone;
                        rental.Email = model.Email;
                        rental.Address = model.Address;
                        rental.Message = model.Message;
                        rental.DateFrom= model.DateFrom;
                        rental.DateTo= model.DateTo;
                        rental.DateShip = model.DateShip;
                        rental.StatusId = model.StatusId;
                    }

                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Sửa thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(model.RentalId))
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
            return View(model);
        }

        // GET: Jobber/JobberRentals/Delete/5
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
            var Chitietdonhang = _context.RentalDetails
                .AsNoTracking()
                .Include(x => x.Vehicle)
                .Include(x => x.Rental)
                .Where(x => x.RentalId == rental.RentalId)
                .OrderBy(x => x.RentalDetailId)
                .ToList();
            ViewBag.ChiTiet = Chitietdonhang;

            return View(rental);
        }

        // POST: Jobber/JobberRentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rentals == null)
            {
                return Problem("Không tìm thấy đơn hàng.");
            }
            var rental = await _context.Rentals.FindAsync(id);

            var rentalDetails = _context.RentalDetails.Include(x => x.Rental).Where(x => x.RentalId == id);
            if (rentalDetails != null)
            {
                foreach(var item in rentalDetails)
                {
                    _context.Remove(item);
                }
                _context.SaveChanges();
            }

            if (rental != null)
            {
                _context.Rentals.Remove(rental);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xoá thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return (_context.Rentals?.Any(e => e.RentalId == id)).GetValueOrDefault();
        }
    }
}
