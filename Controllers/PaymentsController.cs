using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModels.Payment;

namespace QuanLyKhachSan.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == _userManager.GetUserId(User));

            if (booking == null)
            {
                return NotFound();
            }

            var model = new PaymentViewModel
            {
                BookingId = booking.Id,
                RoomName = booking.Room.Name,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                NumberOfGuests = booking.NumberOfGuests
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int bookingId, string paymentMethod)
        {
            try
            {
                Console.WriteLine($"🚀 PROCESS PAYMENT STARTED - Booking: {bookingId}");

                var booking = await _context.Bookings
                    .Include(b => b.Room)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                {
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction("Payment", new { bookingId });
                }

                // Check if already paid
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == bookingId && p.Status == PaymentStatus.Completed);

                if (existingPayment != null)
                {
                    TempData["InfoMessage"] = "This booking has already been paid.";
                    return RedirectToAction("Details", "Bookings", new { id = bookingId });
                }

                // Tạo payment
                var payment = new Payment
                {
                    BookingId = bookingId,
                    Amount = booking.TotalPrice,
                    PaymentMethod = paymentMethod,
                    Status = PaymentStatus.Completed,
                    TransactionId = "PAY_" + DateTime.Now.Ticks.ToString(),
                    PaymentDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // Update booking status
                booking.Status = BookingStatus.Confirmed;
                booking.UpdatedAt = DateTime.UtcNow;

                // Save to database
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ PAYMENT SUCCESS - Redirecting to Invoice: {payment.Id}");

                // QUAN TRỌNG: Redirect đến Invoice với paymentId
                return RedirectToAction("Invoice", new { paymentId = payment.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ PAYMENT ERROR: {ex.Message}");
                TempData["ErrorMessage"] = $"Payment failed: {ex.Message}";
                return RedirectToAction("Payment", new { bookingId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Room)
                .Include(p => p.Booking)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            var model = new InvoiceViewModel
            {
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                BookingId = payment.BookingId,
                RoomName = payment.Booking.Room.Name,
                CheckInDate = payment.Booking.CheckInDate,
                CheckOutDate = payment.Booking.CheckOutDate,
                NumberOfGuests = payment.Booking.NumberOfGuests,
                CustomerName = payment.Booking.User.FirstName + " " + payment.Booking.User.LastName,
                CustomerEmail = payment.Booking.User.Email
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentHistory()
        {
            var userId = _userManager.GetUserId(User);
            var payments = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Room)
                .Where(p => p.Booking.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentHistoryViewModel
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    RoomName = p.Booking.Room.Name,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    PaymentDate = p.PaymentDate,
                    TransactionId = p.TransactionId
                })
                .ToListAsync();

            return View(payments);
        }

        private string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.Ticks.ToString();
        }
    }
}