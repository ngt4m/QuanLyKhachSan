using Microsoft.AspNetCore.Mvc;
using QuanLyKhachSan.Services.Interfaces;

namespace QuanLyKhachSan.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRooms()
        {
            var rooms = await _roomService.GetAvailableRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            var room = await _roomService.GetRoomDetailsAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpGet("{id}/availability")]
        public async Task<IActionResult> CheckAvailability(int id, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            var isAvailable = await _roomService.IsRoomAvailableAsync(id, checkIn, checkOut);
            return Ok(new { available = isAvailable });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRooms([FromQuery] string searchTerm, [FromQuery] string roomType,
                                                   [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var rooms = await _roomService.SearchRoomsAsync(searchTerm, roomType, minPrice, maxPrice);
            return Ok(rooms);
        }
    }
}