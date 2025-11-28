using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhachSan.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Ngày nhận phòng là bắt buộc")]
        [Display(Name = "Ngày nhận phòng")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Ngày trả phòng là bắt buộc")]
        [Display(Name = "Ngày trả phòng")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Số khách")]
        [Range(1, 10, ErrorMessage = "Số khách phải từ 1 đến 10")]
        public int NumberOfGuests { get; set; } = 1;

        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Ghi chú đặc biệt")]
        public string? SpecialRequests { get; set; }

        [Display(Name = "Trạng thái")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Display(Name = "Ngày đặt")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
        public virtual Payment? Payment { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        CheckedOut,
        Cancelled
    }
}
