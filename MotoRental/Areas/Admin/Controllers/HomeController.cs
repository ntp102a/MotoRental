using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MotoRental.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "1")]
    public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
