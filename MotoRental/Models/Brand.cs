using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
