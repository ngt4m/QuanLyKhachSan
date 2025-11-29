using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.Services.Interfaces;
using QuanLyKhachSan.ViewModels.Admin;

namespace QuanLyKhachSan.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<List<ReviewViewModel>> GetReviewsByRoomAsync(int roomId)
        {
            return await _context.Reviews
                .Where(r => r.RoomId == roomId)
                .Include(r => r.User)
                .Include(r => r.Room)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    UserName = r.User.FirstName + " " + r.User.LastName,
                    RoomName = r.Room.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<ReviewViewModel>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Room)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    UserName = r.User.FirstName + " " + r.User.LastName,
                    RoomName = r.Room.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasUserReviewedRoomAsync(string userId, int roomId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.RoomId == roomId);
        }

        public async Task<bool> HasUserStayedInRoomAsync(string userId, int roomId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.UserId == userId &&
                              b.RoomId == roomId &&
                              b.Status == Models.BookingStatus.CheckedOut);
        }

        public async Task<double> GetAverageRatingAsync(int roomId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.RoomId == roomId)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<int> GetReviewCountAsync(int roomId)
        {
            return await _context.Reviews
                .CountAsync(r => r.RoomId == roomId);
        }
    }
}