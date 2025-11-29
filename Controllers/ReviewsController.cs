using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModels.Admin;

namespace QuanLyKhachSan.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int roomId, int rating, string comment)
        {
            var user = await _userManager.GetUserAsync(User);
            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null)
            {
                return NotFound();
            }

            // Check if user has stayed in this room
            var hasBooking = await _context.Bookings
                .AnyAsync(b => b.UserId == user.Id &&
                              b.RoomId == roomId &&
                              b.Status == BookingStatus.CheckedOut);

            if (!hasBooking)
            {
                TempData["ErrorMessage"] = "You can only review rooms you have stayed in.";
                return RedirectToAction("Details", "Rooms", new { id = roomId });
            }

            // Check if user already reviewed this room
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.RoomId == roomId);

            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "You have already reviewed this room.";
                return RedirectToAction("Details", "Rooms", new { id = roomId });
            }

            var review = new Review
            {
                UserId = user.Id,
                RoomId = roomId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Review submitted successfully!";
            return RedirectToAction("Details", "Rooms", new { id = roomId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Review deleted successfully!";
            }

            return RedirectToAction("Reviews", "Admin");
        }
    }
}