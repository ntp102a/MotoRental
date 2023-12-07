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
                    .Include(v => v.Image)
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

        public IActionResult List(int id, int page = 1)
        {
            try
            {
                var pageSize = 10;
                var danhmuc = _context.Brands
                    .AsNoTracking()
                    .SingleOrDefault(x => x.BrandId == id);
                var IsPages = _context.Vehicles
                    .Include(p => p.Image)
                    .Include(p => p.Brand)
                    .Include(p => p.Displacement)
                    .Include(p => p.User)
                    .Where(p => p.BrandId == id) // Lọc sản phẩm dựa trên CategoryId
                    .AsNoTracking()
                    .OrderByDescending(x => x.UpdationDate);
                PagedList<Vehicle> models = new PagedList<Vehicle>(IsPages, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc;
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
                    .Include(v => v.Image)
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
                    .Include(v => v.Image)
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
    }
}
