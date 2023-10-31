using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            RentalDetails = new HashSet<RentalDetail>();
        }

        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? LicensePlate { get; set; }
        public string? Overview { get; set; }
        public byte[]? Image { get; set; }
        public int? PricePerDay { get; set; }
        public int? Kilometers { get; set; }
        public int? Status { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? UpdationDate { get; set; }
        public int? UserId { get; set; }
        public int? BrandId { get; set; }
        public int? DisplacementId { get; set; }

        public virtual Brand? Brand { get; set; }
        public virtual Displacement? Displacement { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<RentalDetail> RentalDetails { get; set; }
    }
}
