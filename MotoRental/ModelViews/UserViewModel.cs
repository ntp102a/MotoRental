﻿namespace MotoRental.ModelViews
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Salt { get; set; }
        public int? RoleId { get; set; }
    }
}
