using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Location
    {
        public Location()
        {
            Users = new HashSet<User>();
        }

        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
