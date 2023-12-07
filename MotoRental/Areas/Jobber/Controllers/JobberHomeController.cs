using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MotoRental.Areas.Jobber.Controllers
{
    [Area("Jobber")]
    [Authorize(Roles = "2")]
    public class JobberHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
