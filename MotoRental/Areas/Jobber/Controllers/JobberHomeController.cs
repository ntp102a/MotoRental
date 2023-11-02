using Microsoft.AspNetCore.Mvc;

namespace MotoRental.Areas.Jobber.Controllers
{
    [Area("Jobber")]
    public class JobberHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
