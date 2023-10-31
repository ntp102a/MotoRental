using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class User
    {
        public User()
        {
            Rentals = new HashSet<Rental>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public int? Phone { get; set; }
        public int? RoleId { get; set; }
        public int? LocationId { get; set; }

        public virtual Location? Location { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
