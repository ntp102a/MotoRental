using Microsoft.AspNetCore.Mvc;
using MotoRental.Models;
using System;

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
        public IActionResult ExportData(int year, int month)
        {
            // Tạo mảng để lưu trữ doanh thu của từng ngày trong tháng
            int[] dailyDataArray = new int[DateTime.DaysInMonth(year, month)];

            var rentalData = _context.Rentals
                .Where(o => o.StatusId == 2 || o.StatusId == 5)
                .Where(o => o.DateFrom.HasValue &&
                            o.DateFrom.Value.Year == year &&
                            o.DateFrom.Value.Month == month)
                .OrderBy(o => o.DateFrom)
                .ToList();

            // Điền giá trị từ dữ liệu thực tế vào mảng
            foreach (var rental in rentalData)
            {
                // Lấy chỉ số của ngày trong mảng (bắt đầu từ 0)
                int dayIndex = rental.DateFrom.Value.Day - 1;

                // Gán giá trị vào mảng
                dailyDataArray[dayIndex] += rental.Price ?? 0;
            }

            // Tạo danh sách đối tượng để trả về
            var dailyDataList = new List<dynamic>();
            for (int i = 0; i < dailyDataArray.Length; i++)
            {
                dailyDataList.Add(new { Date = i + 1, Total = dailyDataArray[i] });
            }

            return Json(new { isEmpty = dailyDataList.Count == 0, data = dailyDataList });
        }



    }
}
