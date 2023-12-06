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
        public async Task<IActionResult> SearchingProductAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                // Trường hợp keyword trống, có thể xử lý hoặc trả về một trang lỗi
                return View("Error");
            }
            // Sử dụng ToListAsync để tránh chặn luồng trong truy vấn async
            var lsProduct = await _context.Vehicles
                .AsNoTracking()
                .Include(p => p.VehicleId)
                .Include(p => p.Overview)
                .Where(x => x.VehicleName.Contains(keyword))
                .ToListAsync();

            return View(lsProduct);
        }
    }
}
