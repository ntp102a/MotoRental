
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoRental.Extension;
using MotoRental.Models;

namespace LaptopShop.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        private readonly Rental_motorbikeContext _context;

        public HeaderCartViewComponent(Rental_motorbikeContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var accountID = User.Identity.GetAccountID();

            if (int.TryParse(accountID, out var userId))
            {
                var cartItems = _context.Carts
                .Include(c => c.Vehicle)
                .Include(c => c.Vehicle.Image)
                .Where(c => c.UserId == userId)
                .Select(c => new Cart
                {
                    Vehicle = c.Vehicle,
                    Quantity = (int)c.Quantity
                })
                .ToList();
                return View(cartItems);
            }
            else
            {
                return View(new List<Cart>());
            }

        }
    }
}
