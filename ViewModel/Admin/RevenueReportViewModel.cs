namespace QuanLyKhachSan.ViewModels.Admin
{
    public class RevenueReportViewModel
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CheckedInBookings { get; set; }
        public int CheckedOutBookings { get; set; }
        public List<RevenueByRoomType> RevenueByRoomTypes { get; set; } = new List<RevenueByRoomType>();
        public List<DailyRevenue> DailyRevenues { get; set; } = new List<DailyRevenue>();
        public List<MonthlyRevenue> MonthlyRevenues { get; set; } = new List<MonthlyRevenue>();
    }

    public class RevenueByRoomType
    {
        public string RoomType { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class DailyRevenue
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
    }
}