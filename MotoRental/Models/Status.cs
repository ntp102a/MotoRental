using System;
using System.Collections.Generic;

namespace MotoRental.Models
{
    public partial class Status
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
}
