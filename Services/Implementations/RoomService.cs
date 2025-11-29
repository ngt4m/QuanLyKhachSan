using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.Services.Interfaces;
using QuanLyKhachSan.ViewModels.Room;

namespace QuanLyKhachSan.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomCardViewModel>> GetAvailableRoomsAsync()
        {
            return await _context.Rooms
                .Where(r => r.IsAvailable)
                .Select(r => new RoomCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description.Length > 100 ? r.Description.Substring(0, 100) + "..." : r.Description,
                    Price = r.Price,
                    Type = r.Type,
                    ImageUrl = r.ImageUrl ?? "/images/rooms/default.jpg",
                    IsAvailable = r.IsAvailable,
                    AverageRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0,
                    ReviewCount = r.Reviews.Count
                })
                .ToListAsync();
        }

        public async Task<RoomDetailsViewModel> GetRoomDetailsAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Reviews)
                .ThenInclude(rev => rev.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return null!;

            return new RoomDetailsViewModel
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Price = room.Price,
                Type = room.Type,
                Capacity = room.Capacity,
                Size = room.Size,
                ImageUrl = room.ImageUrl ?? "/images/rooms/default.jpg",
                IsAvailable = room.IsAvailable,
                Reviews = room.Reviews.Select(rev => new ViewModels.Admin.ReviewViewModel
                {
                    UserName = rev.User.FirstName + " " + rev.User.LastName,
                    Rating = rev.Rating,
                    Comment = rev.Comment,
                    CreatedAt = rev.CreatedAt
                }).ToList(),
                AverageRating = room.Reviews.Any() ? room.Reviews.Average(rev => rev.Rating) : 0
            };
        }

        public async Task<List<RoomCardViewModel>> SearchRoomsAsync(string? searchTerm, string? roomType, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Rooms.Where(r => r.IsAvailable).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Name.Contains(searchTerm) || r.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(roomType))
            {
                query = query.Where(r => r.Type == roomType);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(r => r.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(r => r.Price <= maxPrice.Value);
            }

            return await query
                .Select(r => new RoomCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description.Length > 100 ? r.Description.Substring(0, 100) + "..." : r.Description,
                    Price = r.Price,
                    Type = r.Type,
                    ImageUrl = r.ImageUrl ?? "/images/rooms/default.jpg",
                    IsAvailable = r.IsAvailable,
                    AverageRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0,
                    ReviewCount = r.Reviews.Count
                })
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            return !await _context.Bookings
                .AnyAsync(b => b.RoomId == roomId &&
                              b.Status != BookingStatus.Cancelled &&
                              ((checkIn >= b.CheckInDate && checkIn < b.CheckOutDate) ||
                               (checkOut > b.CheckInDate && checkOut <= b.CheckOutDate) ||
                               (checkIn <= b.CheckInDate && checkOut >= b.CheckOutDate)));
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }
    }
}