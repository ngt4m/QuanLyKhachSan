using QuanLyKhachSan.ViewModels.Admin;

namespace QuanLyKhachSan.ViewModels.Room
{
    public class RoomDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Size { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        public double AverageRating { get; set; }
    }
}