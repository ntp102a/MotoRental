using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }
        public int? VehicleId { get; set; }
        public int? UserId { get; set; }
        public int? Quantity { get; set; }
        public double TotalMoney { get { return (double)(Quantity * Vehicle.PricePerDay.Value); } }

        public virtual User? User { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
    }
}
