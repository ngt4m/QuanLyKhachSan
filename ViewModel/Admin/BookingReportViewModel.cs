using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.ViewModels.Admin
{
    public class BookingReportViewModel
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CheckedInBookings { get; set; }
        public int CheckedOutBookings { get; set; }
        public int CancelledBookings { get; set; }
        public List<BookingStatusCount> StatusCounts { get; set; } = new List<BookingStatusCount>();
        public List<BookingTrend> BookingTrends { get; set; } = new List<BookingTrend>();
        public List<PopularRoom> PopularRooms { get; set; } = new List<PopularRoom>();
    }

    public class BookingStatusCount
    {
        public BookingStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class BookingTrend
    {
        public string Period { get; set; } = string.Empty;
        public int BookingCount { get; set; }
    }

    public class PopularRoom
    {
        public string RoomName { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}