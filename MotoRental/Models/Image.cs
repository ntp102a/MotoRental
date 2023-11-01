using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Image
    {
        public Image()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public int ImageId { get; set; }
        public string? ImageFont { get; set; }
        public string? ImageLeftSide { get; set; }
        public string? ImageRightSide { get; set; }
        public string? ImageBackSide { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
