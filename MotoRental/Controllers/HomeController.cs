﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoRental.Models;
using MotoRental.ModelViews;
using System.Diagnostics;

namespace MotoRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Rental_motorbikeContext _context;

        public HomeController(ILogger<HomeController> logger, Rental_motorbikeContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewVM model = new HomeViewVM();

            var lsProducts = _context.Vehicles
                .AsNoTracking()
                .Include(x => x.Image)
                .OrderByDescending(x => x.UpdationDate)
                .ToList();

            List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();
            var lsBrands = _context.Brands
                .AsNoTracking()
                .OrderByDescending(x => x.BrandId)
                .ToList();

            foreach (var item in lsBrands)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.brand = item;
                productHome.lsVehicles = lsProducts
                    .Where(x => x.BrandId == item.BrandId)
                    .ToList();
                lsProductViews.Add(productHome);
            }
            model.Vehicles = lsProductViews;
            ViewBag.AllProducts = lsProducts;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}