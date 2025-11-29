using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.Services.Interfaces;
using QuanLyKhachSan.ViewModels.Admin;
using QuanLyKhachSan.ViewModels.Booking;

namespace QuanLyKhachSan.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueReportViewModel> GetRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var completedBookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.Status == BookingStatus.CheckedOut &&
                           b.CheckOutDate >= startDate &&
                           b.CheckOutDate <= endDate)
                .ToListAsync();

            var revenueByRoomType = completedBookings
                .GroupBy(b => b.Room.Type)
                .Select(g => new RevenueByRoomType
                {
                    RoomType = g.Key,
                    Revenue = g.Sum(b => b.TotalPrice),
                    BookingCount = g.Count()
                })
                .ToList();

            var dailyRevenues = completedBookings
                .GroupBy(b => b.CheckOutDate.Date)
                .Select(g => new DailyRevenue
                {
                    Date = g.Key,
                    Revenue = g.Sum(b => b.TotalPrice),
                    BookingCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            var monthlyRevenues = completedBookings
                .GroupBy(b => new { b.CheckOutDate.Year, b.CheckOutDate.Month })
                .Select(g => new MonthlyRevenue
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(b => b.TotalPrice),
                    BookingCount = g.Count()
                })
                .OrderBy(m => m.Month)
                .ToList();

            return new RevenueReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = completedBookings.Sum(b => b.TotalPrice),
                TotalBookings = completedBookings.Count,
                ConfirmedBookings = completedBookings.Count(b => b.Status == BookingStatus.Confirmed),
                CheckedInBookings = completedBookings.Count(b => b.Status == BookingStatus.CheckedIn),
                CheckedOutBookings = completedBookings.Count(b => b.Status == BookingStatus.CheckedOut),
                RevenueByRoomTypes = revenueByRoomType,
                DailyRevenues = dailyRevenues,
                MonthlyRevenues = monthlyRevenues
            };
        }

        public async Task<BookingReportViewModel> GetBookingReportAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                .ToListAsync();

            var totalBookings = bookings.Count;
            var statusCounts = bookings
                .GroupBy(b => b.Status)
                .Select(g => new BookingStatusCount
                {
                    Status = g.Key,
                    StatusName = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = totalBookings > 0 ? (g.Count() * 100m / totalBookings) : 0
                })
                .ToList();

            // Generate booking trends (last 12 months)
            var bookingTrends = new List<BookingTrend>();
            for (int i = 11; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                var monthBookings = bookings.Count(b =>
                    b.CreatedAt.Year == month.Year && b.CreatedAt.Month == month.Month);

                bookingTrends.Add(new BookingTrend
                {
                    Period = month.ToString("MMM yyyy"),
                    BookingCount = monthBookings
                });
            }

            var popularRooms = bookings
                .GroupBy(b => new { b.Room.Id, b.Room.Name, b.Room.Type })
                .Select(g => new PopularRoom
                {
                    RoomName = g.Key.Name,
                    RoomType = g.Key.Type,
                    BookingCount = g.Count(),
                    TotalRevenue = g.Sum(b => b.TotalPrice)
                })
                .OrderByDescending(r => r.BookingCount)
                .Take(10)
                .ToList();

            return new BookingReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalBookings = totalBookings,
                PendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending),
                ConfirmedBookings = bookings.Count(b => b.Status == BookingStatus.Confirmed),
                CheckedInBookings = bookings.Count(b => b.Status == BookingStatus.CheckedIn),
                CheckedOutBookings = bookings.Count(b => b.Status == BookingStatus.CheckedOut),
                CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled),
                StatusCounts = statusCounts,
                BookingTrends = bookingTrends,
                PopularRooms = popularRooms
            };
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync()
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var totalBookings = await _context.Bookings.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalRooms = await _context.Rooms.CountAsync();

            var revenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.CheckedOut)
                .SumAsync(b => b.TotalPrice);

            var monthlyRevenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.CheckedOut &&
                           b.CheckOutDate >= monthStart &&
                           b.CheckOutDate <= monthEnd)
                .SumAsync(b => b.TotalPrice);

            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new BookingViewModel
                {
                    Id = b.Id,
                    UserName = b.User.FirstName + " " + b.User.LastName,
                    RoomName = b.Room.Name,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalBookings = totalBookings,
                TotalUsers = totalUsers,
                TotalRooms = totalRooms,
                TotalRevenue = revenue,
                MonthlyRevenue = monthlyRevenue,
                RecentBookings = recentBookings
            };
        }
    }
}