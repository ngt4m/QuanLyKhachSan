using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.Services.Interfaces;
using QuanLyKhachSan.ViewModels.Admin;
using QuanLyKhachSan.ViewModels.Booking;

namespace QuanLyKhachSan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReportService _reportService;
        private readonly IReviewService _reviewService;

        public AdminController(ApplicationDbContext context,
                              UserManager<ApplicationUser> userManager,
                              IReportService reportService,
                              IReviewService reviewService)
        {
            _context = context;
            _userManager = userManager;
            _reportService = reportService;
            _reviewService = reviewService;
        }

        // CHỈ GIỮ LẠI MỘT PHƯƠNG THỨC DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _reportService.GetDashboardStatsAsync();
            return View(stats);
        }

        [HttpGet]
        public async Task<IActionResult> RevenueReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var report = await _reportService.GetRevenueReportAsync(start, end);
            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> BookingReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var report = await _reportService.GetBookingReportAsync(start, end);
            return View(report);
        }

        // Các phương thức khác giữ nguyên...
        public async Task<IActionResult> AdminRooms()
        {
            var rooms = await _context.Rooms
                .Select(r => new RoomViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Type = r.Type,
                    Price = r.Price,
                    IsAvailable = r.IsAvailable,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return View(rooms);
        }

        [HttpGet]
        public IActionResult AddRoom()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(RoomCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Capacity = model.Capacity,
                    Size = model.Size,
                    Type = model.Type,
                    ImageUrl = model.ImageUrl,
                    IsAvailable = model.IsAvailable,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Room added successfully!";
                return RedirectToAction("AdminRooms");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var model = new RoomCreateViewModel
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Price = room.Price,
                Capacity = room.Capacity,
                Size = room.Size,
                Type = room.Type,
                ImageUrl = room.ImageUrl,
                IsAvailable = room.IsAvailable
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoom(RoomCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var room = await _context.Rooms.FindAsync(model.Id);
                if (room == null)
                {
                    return NotFound();
                }

                room.Name = model.Name;
                room.Description = model.Description;
                room.Price = model.Price;
                room.Capacity = model.Capacity;
                room.Size = model.Size;
                room.Type = model.Type;
                room.ImageUrl = model.ImageUrl;
                room.IsAvailable = model.IsAvailable;
                room.UpdatedAt = DateTime.UtcNow;

                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Room updated successfully!";
                return RedirectToAction("AdminRooms");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                // Check if room has bookings
                var hasBookings = await _context.Bookings.AnyAsync(b => b.RoomId == id);

                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Cannot delete room with existing bookings.";
                }
                else
                {
                    _context.Rooms.Remove(room);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Room deleted successfully!";
                }
            }

            return RedirectToAction("AdminRooms");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRoomStatus(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                room.IsAvailable = !room.IsAvailable;
                room.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Room {(room.IsAvailable ? "activated" : "deactivated")} successfully!";
            }

            return RedirectToAction("AdminRooms");
        }

        public async Task<IActionResult> AdminBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
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

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.Status = status;
                booking.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Booking status updated successfully!";
            }

            return RedirectToAction("AdminBookings");
        }

        public async Task<IActionResult> AdminUsers()
        {
            var users = await _context.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && user.Email != User.Identity!.Name)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
            }

            return RedirectToAction("AdminUsers");
        }

        public async Task<IActionResult> Reviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Review deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Review not found.";
            }
            return RedirectToAction("Reviews");
        }
    }
}