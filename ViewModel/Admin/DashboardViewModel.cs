using QuanLyKhachSan.ViewModels.Booking;

namespace QuanLyKhachSan.ViewModels.Admin
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<BookingViewModel> RecentBookings { get; set; } = new List<BookingViewModel>();
    }

    public class RoomViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}