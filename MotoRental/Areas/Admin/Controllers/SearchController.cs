using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoRental.Models;
using System.Data;

namespace MarketWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class SearchController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public SearchController(Rental_motorbikeContext context)
        {
            _context = context;
        }
        // GET: Search/FindProduct
        [HttpPost]
        public IActionResult FindVehicle(string keyword)
        {
            List<Vehicle> ls = new List<Vehicle>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListVehiclesSearchPartialView", null);
            }
            ls = _context.Vehicles
                .AsNoTracking()
                .Include(a => a.Brand)
                .Where(x => x.VehicleName.Contains(keyword))
                .OrderByDescending(x => x.VehicleName)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListVehiclesSearchPartialView", null);
            }
            else
            {
                return PartialView("ListVehiclesSearchPartialView", ls);
            }
        }
    }
}
