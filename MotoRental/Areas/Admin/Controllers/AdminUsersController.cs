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
using MotoRental.Helpper;
using MotoRental.Models;
using MotoRental.ModelViews;
using PagedList.Core;

namespace MotoRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class AdminUsersController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        public INotyfService _notyfService { get; }

        public AdminUsersController(Rental_motorbikeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminUsers
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var IsCustomers = _context.Users
                .AsNoTracking()
                //.Include(x => x.Location)
                .Include(u => u.Role)
                .OrderByDescending(x => x.UserId);
            PagedList<User> models = new PagedList<User>(IsCustomers, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Location)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/AdminUsers/Create
        public IActionResult Create()
        {
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationId");
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Admin/AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                string salt = Utilities.GetRandomKey();
                user.Salt = salt;

                user.Password = (user.Password + salt.Trim()).ToMD5();

                _context.Add(user);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }

        // GET: Admin/AdminUsers/Edit/5
        public async Task<IActionResult> Edit(int? id, UserViewModel model)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                model.UserId = user.UserId;
                model.FullName = user.FullName;
                model.Phone = user.Phone;
                model.Email = user.Email;
                model.Address = user.Address;
                model.Password = user.Password;
                model.Salt = user.Salt;
                model.RoleId = user.RoleId;
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", model.RoleId);
            return View(model);
        }

        // POST: Admin/AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Users.SingleOrDefault(x => x.UserId == model.UserId);
                    if (user != null)
                    {
                        user.FullName = model.FullName;
                        user.Phone = model.Phone;
                        user.Email = model.Email;
                        user.Address = model.Address;
                        user.RoleId = model.RoleId;
                        if (user.Password != (model.Password + user.Salt.Trim()).ToMD5())
                        {
                            string salt = Utilities.GetRandomKey();
                            user.Salt = salt;
                            user.Password = (model.Password + salt.Trim()).ToMD5();
                        }
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                        _notyfService.Success("Sửa thành công");
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.UserId))
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

        // GET: Admin/AdminUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Location)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Rental_motorbikeContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);

            var vehicle = _context.Vehicles.Include(x => x.User).Where(x => x.UserId == id).ToList();

            if (vehicle != null)
            {
                foreach(var item in vehicle)
                {
                    var carts = _context.Carts.Include(x => x.Vehicle).Where(x => x.VehicleId == item.VehicleId).ToList();
                    if (carts != null)
                    {
                        foreach (var cart in carts)
                        {
                            _context.Carts.Remove(cart);
                        }
                        _context.SaveChanges();
                    }

                    var orderdetails = _context.RentalDetails.Include(x => x.Vehicle).Where(x => x.VehicleId == item.VehicleId).ToList();
                    if (orderdetails != null)
                    {
                        foreach (var orderdetail in orderdetails)
                        {
                            _context.RentalDetails.Remove(orderdetail);
                        }
                        _context.SaveChanges();
                    }

                    var image = await _context.Images.FindAsync(item?.ImageId);
                    if (image != null)
                    {
                        _context.Images.Remove(image);
                    }
                    _context.Vehicles.Remove(item);
                }
                _context.SaveChanges();
            }

            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xoá thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
