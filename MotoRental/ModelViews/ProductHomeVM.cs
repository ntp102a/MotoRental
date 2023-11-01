using MotoRental.Models;

namespace MotoRental.ModelViews
{
    public class ProductHomeVM
    {
        public Brand brand { get; set; }
        public List<Vehicle> lsVehicles { get; set; }
    }
}
