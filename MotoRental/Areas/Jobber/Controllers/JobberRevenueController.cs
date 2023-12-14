using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MotoRental.Extension;
using MotoRental.Models;
using System.Linq;

namespace MotoRental.Areas.Jobber.Controllers
{
    [Area("Jobber")]
    [Authorize(Roles = "2")]
    public class JobberRevenueController : Controller
    {
        private readonly Rental_motorbikeContext _context;

        public JobberRevenueController(Rental_motorbikeContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var accountID = User.Identity.GetAccountID();

            List<Rental> lsOrders = new List<Rental>();

            if (int.TryParse(accountID, out var userId))
            {
                var orderDetails = _context.RentalDetails
                    .AsNoTracking()
                .Include(x => x.Vehicle)
                .Include(x => x.Rental)
                .Where(x => x.Vehicle.UserId == userId)
                .ToList();
                foreach (var item in orderDetails)
                {
                    var lsOrder = _context.Rentals
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Status)
                    .FirstOrDefault(x => x.RentalId == item.RentalId);
                    lsOrders.Add(lsOrder);
                }

                var totalOrders = orderDetails.Where(x => x.Vehicle.UserId == userId).Count();

                var totalUser = lsOrders
                    .GroupBy(o => o.UserId)
                    .Select(g => g.Key)
                    .Count();

                ViewBag.TotalOrders = totalOrders;
                ViewBag.TotalOrdersUser = totalUser;
                return View();
            }
            else
            {
                return View();
            }
        }
        public IActionResult ExportData(int year, int month)
        {
            // Tạo mảng để lưu trữ doanh thu của từng ngày trong tháng
            int[] dailyDataArray = new int[DateTime.DaysInMonth(year, month)];

            var rentalData = _context.Rentals
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
