using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Status
    {
        public Status()
        {
            Rentals = new HashSet<Rental>();
        }

        public int StatusId { get; set; }
        public string? StatusName { get; set; }

        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
