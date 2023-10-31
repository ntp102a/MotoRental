using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Model
    {
        public int Id { get; set; }
        public decimal? DailyHireRate { get; set; }
        public string? Name { get; set; }
        public int? ManufacturerId { get; set; }
    }
}
