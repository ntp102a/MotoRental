using MotoRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotoRental.ModelViews
{
    public class XemDonHang
    {
        public Rental DonHang { get; set; }
        public List<RentalDetail> ChiTietDonHang { get; set; }
    }
}
