using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.Services.Interfaces;
using QuanLyKhachSan.ViewModels.Booking;

namespace QuanLyKhachSan.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoomService _roomService;

        public BookingService(ApplicationDbContext context, IRoomService roomService)
        {
            _context = context;
            _roomService = roomService;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            // Check room availability
            var isAvailable = await _roomService.IsRoomAvailableAsync(
                booking.RoomId, booking.CheckInDate, booking.CheckOutDate);

            if (!isAvailable)
            {
                throw new InvalidOperationException("Room is not available for the selected dates.");
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<BookingListViewModel>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Room)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingListViewModel
                {
                    Id = b.Id,
                    RoomName = b.Room.Name,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> CancelBookingAsync(int bookingId, string userId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null || booking.Status == BookingStatus.Cancelled)
                return false;

            // Only allow cancellation if check-in is at least 24 hours away
            if (booking.CheckInDate <= DateTime.Now.AddHours(24))
                return false;

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.CheckInDate >= startDate && b.CheckOutDate <= endDate)
                .Include(b => b.Room)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<decimal> CalculateBookingTotalAsync(int roomId, DateTime checkIn, DateTime checkOut, int numberOfGuests)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                throw new ArgumentException("Room not found");

            var numberOfDays = (checkOut - checkIn).Days;
            return room.Price * numberOfDays;
        }
    }
}