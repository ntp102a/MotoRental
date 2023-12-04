﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using MotoRental.Extension;
using MotoRental.Models;

namespace LaptopShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        public INotyfService _notyfService { get; }
        public ShoppingCartController(Rental_motorbikeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [Authorize]
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddtoCart(int vehicleID, int? amount)
        {
            try
            {
                var accountID = User.Identity.GetAccountID();

                if (int.TryParse(accountID, out var userId))
                {
                    var cartItem = _context.Carts
                        .FirstOrDefault(c => c.UserId == userId && c.VehicleId == vehicleID);

                    if (cartItem != null)
                    {
                        cartItem.Quantity += amount ?? 1;
                    }
                    else
                    {
                        var newCartItem = new Cart
                        {
                            UserId = userId,
                            VehicleId = vehicleID,
                            Quantity = amount ?? 1
                        };

                        _context.Carts.Add(newCartItem);
                    }

                    _context.SaveChanges();
                    _notyfService.Success("Thêm vào giỏ hàng thành công");
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Lỗi khi xử lý giỏ hàng" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int vehicleID, int? amount)
        {
            try
            {
                var accountID = User.Identity.GetAccountID();

                if (int.TryParse(accountID, out var userId))
                {
                    var cartItem = _context.Carts
                        .FirstOrDefault(c => c.UserId == userId && c.VehicleId == vehicleID);

                    if (cartItem != null && amount.HasValue)
                    {
                        cartItem.Quantity = amount.Value;
                        _context.SaveChanges();
                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false, error = "Lỗi khi cập nhật giỏ hàng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int vehicleID)
        {
            try
            {
                var accountID = User.Identity.GetAccountID();

                if (int.TryParse(accountID, out var userId))
                {
                    var cartItem = _context.Carts
                        .FirstOrDefault(c => c.UserId == userId && c.VehicleId == vehicleID);

                    if (cartItem != null)
                    {
                        _context.Carts.Remove(cartItem);
                        _context.SaveChanges();
                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false, error = "Lỗi khi xóa sản phẩm khỏi giỏ hàng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize]
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            var userId = int.Parse(User.Identity.GetAccountID());
            var gioHang = _context.Carts
                .Include(c => c.Vehicle)
                .Include(c => c.User)
                .Include(c => c.Vehicle.Image)
                .Where(c => c.UserId == userId).ToList();
            var cartItems = gioHang.Select(c => new Cart
            {
                Vehicle = c.Vehicle,
                Quantity = c.Quantity,
                User = c.User,
            }).ToList();

            return View(cartItems);
        }
    }
}
