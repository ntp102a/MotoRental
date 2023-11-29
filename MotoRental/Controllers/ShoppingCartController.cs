using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoRental.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using MotoRental.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace MotoRental.Controllers
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

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddtoCart(int vehicleID, int? amount)
        {
            List<CartItem> gioHang = GioHang;
            try
            {
                CartItem item = GioHang.SingleOrDefault(x => x.vehicle.VehicleId == vehicleID);
                if (item != null)
                {
                    if (amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    else
                    {
                        item.amount++;
                    }
                }
                else
                {
                    Vehicle xe = _context.Vehicles.SingleOrDefault(x => x.VehicleId == vehicleID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        vehicle = xe
                    };
                    gioHang.Add(item);
                }

                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notyfService.Success("Thêm sản phẩm thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int vehicleID, int? amount)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(x => x.vehicle.VehicleId == vehicleID);
                    if (item != null && amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int vehicleID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(x => x.vehicle.VehicleId == vehicleID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
             catch
            {
                return Json(new { success = false });
            }
        }

        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
