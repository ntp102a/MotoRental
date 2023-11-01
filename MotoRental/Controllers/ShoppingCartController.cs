using Microsoft.AspNetCore.Mvc;

namespace MotoRental.Controllers
{
    public class ShoppingCartController : Controller
    {
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
