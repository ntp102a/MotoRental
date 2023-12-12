using MotoRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LaptopShop.Areas.Admin.Controllers
{
    [Area("Jobber")]
    [Authorize(Roles = "2")]
    public class SearchController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public SearchController(Rental_motorbikeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindVehicle(string keyword)
        {
            List<Vehicle> ls = new List<Vehicle>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListVehiclesSearchPartial", null);
            }
            ls = _context.Vehicles.AsNoTracking()
                                  .Include(a => a.Brand)
                                  .Where(x => x.VehicleName.Contains(keyword))
                                  .OrderByDescending(x => x.VehicleName)
                                  .Take(10)
                                  .ToList();
            if (ls == null)
            {
                return PartialView("ListVehiclesSearchPartial", null);
            }
            else
            {
                return PartialView("ListVehiclesSearchPartial", ls);
            }
        }
    }
}
