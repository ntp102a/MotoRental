using MotoRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace MotoRental.ModelViews
{
    public class MuaHangVM
    {
        public int UserId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        public int Phone { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Địa chỉ nhận hàng")]
        public string Address { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        public string Note { get; set; }
    }
}
