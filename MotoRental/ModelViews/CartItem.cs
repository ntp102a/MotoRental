using MotoRental.Models;

namespace MotoRental.ModelViews
{
    public class CartItem
    {
        public Vehicle vehicle { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * vehicle.PricePerDay.Value;
    }
}
