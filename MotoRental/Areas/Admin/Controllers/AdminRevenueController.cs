using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRental.Models;

using System;

using System.Data;


namespace MotoRental.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "1")]
    public class AdminRevenueController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public AdminRevenueController(Rental_motorbikeContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var totalMoneyByYear = _context.Rentals
                .Where(o => o.StatusId.HasValue)
                .Sum(o => o.Price);

            var totalMoneyByMonth = _context.Rentals
                .Where(o => o.StatusId.HasValue && o.DateFrom.Value.Year == 2023 && o.DateFrom.Value.Month == 12)
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


            ViewBag.TotalSum = totalMoneyByYear;
            ViewBag.TotalMonth = totalMoneyByMonth;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalOrdersUser = totalUser;
            return View();
        }
        public IActionResult ExportData(int year, int month)
        {
            // Tạo mảng để lưu trữ doanh thu của từng ngày trong tháng
            int[] dailyDataArray = new int[DateTime.DaysInMonth(year, month)];

            var rentalData = _context.Rentals
                .Where(o => o.Status.StatusName == "Đã thanh toán" || o.Status.StatusName == "Đã nhận hàng")
                .Where(o => o.DateFrom.HasValue &&
                            o.DateFrom.Value.Year == year &&
                            o.DateFrom.Value.Month == month)
                .OrderBy(o => o.DateFrom)
                .ToList();

            int totalRevenueInMonth = 0; // Tổng giá trị của tất cả các ngày

            // Điền giá trị từ dữ liệu thực tế vào mảng
            foreach (var rental in rentalData)
            {
                int dayIndex = rental.DateFrom.Value.Day - 1;
                dailyDataArray[dayIndex] += rental.Price ?? 0;

                // Cập nhật tổng giá trị của tất cả các ngày
                totalRevenueInMonth += rental.Price ?? 0;
            }
            //Tổng doanh thu của năm 
            int totalRevenueInYear = _context.Rentals
                            .Where(o => o.Status.StatusName == "Đã thanh toán" || o.Status.StatusName == "Đã nhận hàng")
                            .Where(o => o.DateFrom.HasValue && o.DateFrom.Value.Year == year)
                            .Sum(o => o.Price) ?? 0;
            // Tạo danh sách đối tượng để trả về
            var dailyDataList = new List<dynamic>();
            for (int i = 0; i < dailyDataArray.Length; i++)
            {
                dailyDataList.Add(new { Date = i + 1, Total = dailyDataArray[i] });
            }
            return Json(new { isEmpty = dailyDataList.Count == 0, data = dailyDataList, totalRevenueInMonth, totalRevenueInYear }); 
        }




    }
}
