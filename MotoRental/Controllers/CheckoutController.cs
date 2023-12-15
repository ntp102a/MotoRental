using AspNetCoreHero.ToastNotification.Abstractions;
using BraintreeHttp;
using MotoRental.Extension;
using MotoRental.Models;
using MotoRental.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoRental.Extension;
using MotoRental.Models;
using PayPal.Core;
using PayPal.v1.Payments;
using System.Diagnostics;

namespace MotoRental.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly Rental_motorbikeContext _context;
        private readonly string _clientId;
        private readonly string _clientSecret;
        public double TyGiaUSD = 24535;
        public INotyfService _notyfService { get; }
        public CheckoutController(Rental_motorbikeContext context, INotyfService notyfService, IConfiguration config)
        {
            _context = context;
            _notyfService = notyfService;
            _clientId = config["PayPalSettings:clientId"];
            _clientSecret = config["PayPalSettings:clientSecret"];
        }

        public List<Cart> GioHang
        {
            get
            {
                var accountID = User.Identity.GetAccountID();

                if (!string.IsNullOrEmpty(accountID) && int.TryParse(accountID, out var userId))
                {
                    var gioHang = _context.Carts
                        .Include(c => c.Vehicle)
                        .Include(c => c.User)
                        .Where(c => c.UserId == userId)
                        .ToList();

                    var cartItems = gioHang.Select(c => new Cart
                    {
                        Vehicle = c.Vehicle,
                        Quantity = c.Quantity,
                        User = c.User,
                    }).ToList();

                    if (cartItems == default(List<Cart>))
                    {
                        cartItems = new List<Cart>();
                    }

                    return cartItems;
                }

                return new List<Cart>();
            }
        }

        //GET: Checkout/Index
        [Authorize]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            try
            {
                var accountID = User.Identity.GetAccountID();

                if (int.TryParse(accountID, out var userId))
                {
                    var model = new MuaHangVM();
                    var khachhang = _context.Users
                        .AsNoTracking()
                        .SingleOrDefault(x => x.UserId == userId);

                    if (khachhang != null)
                    {
                        model.UserId = khachhang.UserId;
                        model.FullName = khachhang.FullName;
                        model.Email = khachhang.Email;
                        model.Phone = khachhang.Phone;
                        model.Address = khachhang.Address;
                    }

                    // Lấy giỏ hàng từ CSDL
                    var cart = _context.Carts
                        .Include(c => c.Vehicle)
                        .Where(c => c.UserId == userId)
                        .Select(c => new Cart
                        {
                            Vehicle = c.Vehicle,
                            Quantity = (int)c.Quantity
                        })
                        .ToList();

                    ViewBag.GioHang = cart;
                    return View(model);
                }

                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu không có tài khoản
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("checkout.html", Name = "Index")]
        public IActionResult Index(MuaHangVM muahang)
        {
            try
            {
                var accountID = User.Identity.GetAccountID();

                if (int.TryParse(accountID, out var userId))
                {
                    var cart = _context.Carts
                        .Include(c => c.Vehicle)
                        .Where(c => c.UserId == userId)
                        .ToList();

                    var khachhang = _context.Users
                        .SingleOrDefault(x => x.UserId == userId);

                    if (khachhang != null)
                    {
                        khachhang.Address = muahang.Address;
                        _context.Update(khachhang);
                        _context.SaveChanges();
                    }

                    var donhang = new Models.Rental
                    {
                        UserId = userId,
                        //Address = muahang.Address,
                        //Phone = muahang.Phone,
                        DateFrom = muahang.DateFrom,
                        DateTo = muahang.DateTo,
                        Message = muahang.Note,
                        StatusId = 1,
                        Price = Convert.ToInt32(cart.Sum(x => x.TotalMoney))
                    };

                    _context.Add(donhang);
                    _context.SaveChanges();

                    foreach (var item in cart)
                    {
                        var orderDetail = new RentalDetail
                        {
                            RentalId = donhang.RentalId,
                            VehicleId = item.VehicleId,
                            Quantity = item.Quantity,
                            CreateDate = DateTime.Now,
                            TotalPrice = item.Vehicle.PricePerDay
                        };

                        _context.Add(orderDetail);

                        var product = _context.Vehicles.Find(item.VehicleId);
                        if (product != null)
                        {
                            _context.Vehicles.Update(product);
                        }
                    }

                    _context.SaveChanges();
                    HttpContext.Session.Remove("GioHang");
                    _notyfService.Success("Đặt hàng thành công");
                    return RedirectToAction("Success");
                }

                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu không có tài khoản
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var accountID = User.Identity.GetAccountID();

                if (!string.IsNullOrEmpty(accountID) && int.TryParse(accountID, out var userId))
                {
                    var khachhang = _context.Users
                        .AsNoTracking()
                        .SingleOrDefault(x => x.UserId == userId);

                    var donhang = _context.Rentals
                        .Where(x => x.UserId == userId)
                        .OrderByDescending(x => x.DateFrom)
                        .FirstOrDefault();

                    if (donhang != null)
                    {
                        MuaHangSuccessVM successVM = new MuaHangSuccessVM
                        {
                            FullName = khachhang.FullName,
                            DonHangID = donhang.RentalId,
                            Phone = khachhang.Phone,
                            Address = khachhang.Address
                        };

                        var cartItems = _context.Carts
                            .Where(c => c.UserId == userId)
                            .ToList();

                        _context.Carts.RemoveRange(cartItems);
                        _context.SaveChanges();

                        return View(successVM);
                    }
                }

                return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Route("dat-hang-khong-thanh-cong.html", Name = "Fail")]
        public IActionResult Fail()
        {
            //Tạo đơn hàng trong database với trạng thái thanh toán là "Chưa thanh toán"
            //Xóa session
            return View();
        }

        [Authorize]
        public async Task<IActionResult> PaypalCheckout(MuaHangVM muahang)
        {
            var environment = new SandboxEnvironment(_clientId, _clientSecret);
            var client = new PayPalHttpClient(environment);
            DateTime startDate = Convert.ToDateTime(muahang.DateFrom);
            DateTime endDate = Convert.ToDateTime(muahang.DateTo);
            TimeSpan time = endDate - startDate;

            double totalDate = time.TotalDays;
            double totalDates = time.Days;

            double roundedValue = 0;
            if ((totalDate - totalDates) >= 0.5)
            {
                roundedValue = (int)Math.Round(totalDate);
            }
            else
            {
                roundedValue = (totalDate * 10) / 10;
            }

            try
            {
                var itemList = new ItemList()
                {
                    Items = new List<Item>()
                };

                var total = Math.Round(GioHang.Sum(p => p.TotalMoney * roundedValue) / TyGiaUSD, 2);

                foreach (var item in GioHang)
                {
                    itemList.Items.Add(new Item()
                    {
                        Name = item.Vehicle.VehicleName,
                        Currency = "USD",
                        Quantity = item.Quantity.ToString(),
                        Sku = "sku",
                        Tax = "0",
                        Price = Math.Round(item.TotalMoney * roundedValue / TyGiaUSD, 2).ToString()
                    });
                }

                var paypalOrderId = DateTime.Now.Ticks;
                var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                var payment = new Payment()
                {
                    Intent = "sale",
                    Transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Amount = new Amount()
                    {
                        Total = total.ToString(),
                        Currency = "USD",
                        Details = new AmountDetails
                        {
                            Tax = "0",
                            Shipping = "0",
                            Subtotal = total.ToString(),
                        }
                    },
                    ItemList = itemList,
                    Description = $"Invoice #{paypalOrderId}",
                    InvoiceNumber = paypalOrderId.ToString()
                }
            },
                    RedirectUrls = new RedirectUrls()
                    {
                        CancelUrl = $"{hostname}/dat-hang-khong-thanh-cong.html",
                        ReturnUrl = $"{hostname}/dat-hang-thanh-cong.html"
                    },
                    Payer = new Payer()
                    {
                        PaymentMethod = "paypal"
                    }
                };

                PaymentCreateRequest request = new PaymentCreateRequest();
                request.RequestBody(payment);

                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                var links = result.Links.GetEnumerator();
                string paypalRedirectUrl = null;

                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        paypalRedirectUrl = lnk.Href;
                    }
                }

                #region Update vào csdl
                var accountID = User.Identity.GetAccountID();

                if (!string.IsNullOrEmpty(accountID) && int.TryParse(accountID, out var userId))
                {
                    var khachhang = _context.Users
                        .AsNoTracking()
                        .SingleOrDefault(x => x.UserId == userId);

                    var cartItems = _context.Carts
                        .Include(c => c.Vehicle)
                        .Where(c => c.UserId == userId)
                        .ToList();

                    //DateTime startDate = Convert.ToDateTime(muahang.DateFrom);
                    //DateTime endDate = Convert.ToDateTime(muahang.DateTo);
                    //TimeSpan time = endDate - startDate;

                    //double totalDate = time.TotalDays;
                    //double totalDates = time.Days;

                    //double roundedValue = 0;
                    //if ( (totalDate - totalDates) >= 0.5)
                    //{
                    //    roundedValue = (int)Math.Round(totalDate);
                    //}
                    //else
                    //{
                    //    roundedValue = (totalDate * 10) / 10;
                    //}

                    if (cartItems.Any())
                    {
                        var donhang = new Rental
                        {
                            UserId = userId,
                            Email = khachhang.Email,
                            Address = muahang.Address,
                            Phone = muahang.Phone,
                            DateFrom= muahang.DateFrom,
                            DateTo= muahang.DateTo,
                            Message = muahang.Note,
                            RentalName = muahang.FullName,
                            StatusId = 1,
                            Price = Convert.ToInt32(cartItems.Sum(x => x.TotalMoney) * roundedValue)
                        };
                        
                        _context.Rentals.Add(donhang);
                        _context.SaveChanges();


                        foreach (var item in cartItems)
                        {
                            var orderDetail = new RentalDetail
                            {
                                RentalId = donhang.RentalId,
                                VehicleId = item.VehicleId,
                                Quantity = item.Quantity,
                                NumberDate = (decimal?)roundedValue,
                                TotalPrice = Convert.ToInt32(item?.Vehicle.PricePerDay * item.Quantity * roundedValue)
                            };

                            _context.RentalDetails.Add(orderDetail);

                            var product = _context.Vehicles.Find(item.VehicleId);
                            product.Status = 1;
                            _context.Vehicles.Update(product);
                        }

                        _context.Carts.RemoveRange(cartItems);
                        _context.SaveChanges();
                    }
                }
                #endregion

                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                return Redirect("/dat-hang-khong-thanh-cong.html");
            }
        }
    }
}
