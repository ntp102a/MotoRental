using AspNetCoreHero.ToastNotification.Abstractions;
using MotoRental.Models;
using MotoRental.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MotoRental.Controllers
{
    public class DonHangController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        public INotyfService _notyfService { get; }
        public DonHangController(Rental_motorbikeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        //GET: DonHang/Details/5
        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Accounts");
                var khachhang = _context.Users
                    .AsNoTracking()
                    .SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));

                if (khachhang == null) return NotFound();
                var donhang= await _context.Rentals
                    .Include(x => x.Status.StatusName)
                    .FirstOrDefaultAsync(x => x.RentalId== id && Convert.ToInt32(taikhoanID) == x.UserId);
                if (donhang == null) return NotFound();

                var chitietdonhang = _context.RentalDetails
                    .AsNoTracking()
                    .Include(x => x.Vehicle)
                    .Where(x => x.RentalId == id)
                    .OrderBy(x => x.RentalDetailId)
                    .ToList();
                XemDonHang donHang = new XemDonHang();
                donHang.DonHang = donhang;
                donHang.ChiTietDonHang = chitietdonhang;
                return PartialView("Details", donHang);
            }
            catch
            {
                return NotFound();
            }
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
