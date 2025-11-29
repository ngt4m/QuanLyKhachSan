using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModels.Room;

namespace QuanLyKhachSan.Services.Interfaces
{
    public interface IRoomService
    {
        Task<List<RoomCardViewModel>> GetAvailableRoomsAsync();
        Task<RoomDetailsViewModel> GetRoomDetailsAsync(int id);
        Task<List<RoomCardViewModel>> SearchRoomsAsync(string searchTerm, string roomType, decimal? minPrice, decimal? maxPrice);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
        Task<List<Room>> GetAllRoomsAsync();
    }
}