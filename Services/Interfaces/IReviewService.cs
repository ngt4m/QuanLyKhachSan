using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModels.Admin;

namespace QuanLyKhachSan.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(Review review);
        Task<List<ReviewViewModel>> GetReviewsByRoomAsync(int roomId);
        Task<List<ReviewViewModel>> GetAllReviewsAsync();
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<bool> HasUserReviewedRoomAsync(string userId, int roomId);
        Task<bool> HasUserStayedInRoomAsync(string userId, int roomId);
        Task<double> GetAverageRatingAsync(int roomId);
        Task<int> GetReviewCountAsync(int roomId);
    }
}