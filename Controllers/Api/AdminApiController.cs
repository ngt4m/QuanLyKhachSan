using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers.Api
{
    [Route("admin/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var totalBookings = await _context.Bookings.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalRooms = await _context.Rooms.CountAsync();
            var revenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.CheckedOut)
                .SumAsync(b => b.TotalPrice);

            return Ok(new
            {
                totalBookings,
                totalUsers,
                totalRooms,
                totalRevenue = revenue
            });
        }
    }
}