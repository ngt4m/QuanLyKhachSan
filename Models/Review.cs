using System.ComponentModel.DataAnnotations;

namespace QuanLyKhachSan.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Bình luận là bắt buộc")]
        [StringLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự")]
        [Display(Name = "Bình luận")]
        public string Comment { get; set; } = string.Empty;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Trạng thái")]
        public ReviewStatus Status { get; set; } = ReviewStatus.Active;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        public int? BookingId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
        public virtual Booking? Booking { get; set; }
    }

    public enum ReviewStatus
    {
        Active,
        Hidden
    }
}
