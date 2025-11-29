using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModels.Booking;

namespace QuanLyKhachSan.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || !room.IsAvailable)
            {
                return NotFound();
            }

            var model = new BookingViewModel
            {
                RoomId = roomId,
                RoomName = room.Name,
                CheckInDateInput = DateTime.Today.AddDays(1),
                CheckOutDateInput = DateTime.Today.AddDays(2),
                NumberOfGuests = 1
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var room = await _context.Rooms.FindAsync(model.RoomId);
                if (room == null || !room.IsAvailable)
                {
                    ModelState.AddModelError("", "Room is not available.");
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                var numberOfDays = (model.CheckOutDateInput - model.CheckInDateInput).Days;

                if (numberOfDays <= 0)
                {
                    ModelState.AddModelError("", "Check-out date must be after check-in date.");
                    return View(model);
                }

                var booking = new Booking
                {
                    UserId = user!.Id,
                    RoomId = model.RoomId,
                    CheckInDate = model.CheckInDateInput,
                    CheckOutDate = model.CheckOutDateInput,
                    NumberOfGuests = model.NumberOfGuests,
                    TotalPrice = room.Price * numberOfDays,
                    Status = BookingStatus.Pending,
                    SpecialRequests = model.SpecialRequests,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", new { id = booking.Id });
            }

            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null || booking.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            var model = new BookingConfirmationViewModel
            {
                BookingId = booking.Id,
                RoomName = booking.Room.Name,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                NumberOfGuests = booking.NumberOfGuests,
                Status = booking.Status
            };

            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == userId)
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

            return View(bookings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null || booking.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            var model = new BookingDetailsViewModel
            {
                Id = booking.Id,
                RoomName = booking.Room.Name,
                RoomType = booking.Room.Type,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NumberOfGuests = booking.NumberOfGuests,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                SpecialRequests = booking.SpecialRequests,
                CreatedAt = booking.CreatedAt
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == _userManager.GetUserId(User));

            if (booking == null)
            {
                return NotFound();
            }

            //Only allow cancellation if check -in is at least 24 hours away
            if (booking.CheckInDate <= DateTime.Now.AddHours(24))
            {
                TempData["ErrorMessage"] = "Cannot cancel booking within 24 hours of check-in.";
                return RedirectToAction("Details", new { id });
            }

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking cancelled successfully!";
            return RedirectToAction("Details", new { id });
        }
    }
}