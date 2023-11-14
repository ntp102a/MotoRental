using Microsoft.AspNetCore.Mvc;
using MotoRental.Models;

namespace MotoRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminRevenueController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public AdminRevenueController(Rental_motorbikeContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var totalMoney = _context.Rentals
                .Where(o => o.StatusId == 2 || o.StatusId == 5)
                .Sum(o => o.Price);

            var totalOrders = _context.Rentals.Count();

            var totalUser = _context.Rentals
                .GroupBy(o => o.UserId)
                .Select(g => g.Key)
                .Count();

            #region Doanh thu theo tháng
            var monthlyData = _context.Rentals
                .OrderBy(o => o.DateFrom)
                .GroupBy(o => new { Month = o.DateFrom.Value.Month, Year = o.DateFrom.Value.Year })
                .Select(g => new { MonthYear = $"{g.Key.Month:00}/{g.Key.Year}", Total = g.Sum(o => o.Price) });
            ViewBag.MonthlyData = monthlyData;
            #endregion

            ViewBag.TotalSum = totalMoney;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalOrdersUser = totalUser;
            return View();
        }
    }
}
