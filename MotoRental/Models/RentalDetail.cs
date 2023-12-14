using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class RentalDetail
    {
        public int RentalDetailId { get; set; }
        public int? RentalId { get; set; }
        public int? VehicleId { get; set; }
        public int? Quantity { get; set; }
        public decimal? NumberDate { get; set; }
        public int? TotalPrice { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual Rental? Rental { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
    }
}
