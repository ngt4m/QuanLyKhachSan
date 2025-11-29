using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModels.Booking;

namespace QuanLyKhachSan.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> GetBookingByIdAsync(int id);
        Task<List<BookingListViewModel>> GetUserBookingsAsync(string userId);
        Task<bool> CancelBookingAsync(int bookingId, string userId);
        Task<List<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateBookingTotalAsync(int roomId, DateTime checkIn, DateTime checkOut, int numberOfGuests);
    }
}