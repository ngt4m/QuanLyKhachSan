using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Services.Interfaces;
using QuanLyKhachSan.ViewModels.Room;

namespace QuanLyKhachSan.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReviewService _reviewService;

        public RoomsController(ApplicationDbContext context, IReviewService reviewService)
        {
            _context = context;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var reviews = await _reviewService.GetReviewsByRoomAsync(id);
            var averageRating = await _reviewService.GetAverageRatingAsync(id);
            var reviewCount = await _reviewService.GetReviewCountAsync(id);

            var viewModel = new RoomDetailsViewModel
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
                Reviews = reviews,
                AverageRating = averageRating
            };

            return View(viewModel);
        }
    }
}