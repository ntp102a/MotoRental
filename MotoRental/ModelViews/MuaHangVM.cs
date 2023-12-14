﻿using MotoRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace MotoRental.ModelViews
{
    public class MuaHangVM
    {
        public int UserId { get; set; }
        [RegularExpression(@"^[^\d!@#$%^&*()_+]+(?: [^\d!@#$%^&*()_+]+)*$", ErrorMessage = "Tên không hợp lệ")]
        public string? FullName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }
        [RegularExpression(@"^[^!@#$%^&*()+=[\]{};:'""|<>?`~]*$", ErrorMessage = "Địa chỉ không hợp lệ")]
        public string? Address { get; set; }
        public string? Note { get; set; }
    }
}
