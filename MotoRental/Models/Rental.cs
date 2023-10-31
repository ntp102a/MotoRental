using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Rental
    {
        public Rental()
        {
            RentalDetails = new HashSet<RentalDetail>();
        }

        public int RentalId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? DateShip { get; set; }
        public string? Message { get; set; }
        public int? Price { get; set; }
        public int? Status { get; set; }
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<RentalDetail> RentalDetails { get; set; }
    }
}
