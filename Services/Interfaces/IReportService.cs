using QuanLyKhachSan.ViewModels.Admin;

namespace QuanLyKhachSan.Services.Interfaces
{
    public interface IReportService
    {
        Task<RevenueReportViewModel> GetRevenueReportAsync(DateTime startDate, DateTime endDate);
        Task<BookingReportViewModel> GetBookingReportAsync(DateTime startDate, DateTime endDate);
        Task<DashboardViewModel> GetDashboardStatsAsync();
    }
}