using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Displacement
    {
        public Displacement()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public int DisplacementId { get; set; }
        public string? DisplacementName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
