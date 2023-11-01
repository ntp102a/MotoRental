using Microsoft.AspNetCore.Mvc;

namespace MotoRental.Controllers
{
    public class CheckoutController : Controller
    {
        [Route("checkout.html", Name = "Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            return View();
        }
    }
}
