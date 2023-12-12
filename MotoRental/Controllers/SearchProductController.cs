using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoRental.Models;

namespace MotoRental.Controllers
{
    public class SearchProductController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public SearchProductController(Rental_motorbikeContext context)
        {
            _context = context;
        }
        public IActionResult SearchingProduct(string keyword)
        {
            var lsProduct = _context.Vehicles
                .AsNoTracking()
                .Include(p => p.Image)
                .Where(x => x.VehicleName.Contains(keyword))
                .ToList(); // Lấy danh sách sản phẩm thay vì sử dụng ToPagedList

            return View(lsProduct);
        }
    }
}
