namespace MotoRental.ModelViews
{
    public class RentalViewModel
    {
        public int RentalId { get; set; }
        public string? RentalName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Message { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set;}
        public DateTime? DateShip { get; set;}
        public int? Price { get; set; }
        public int? StatusId { get; set; }
        public int? UserId { get; set; }


    }
}
