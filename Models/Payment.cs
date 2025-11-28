using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhachSan.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        [Display(Name = "Số tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.PayAtHotel;

        [Display(Name = "Trạng thái thanh toán")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Display(Name = "Ngày thanh toán")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Mã giao dịch")]
        public string? TransactionId { get; set; }

        public virtual Booking Booking { get; set; } = null!;
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        PayAtHotel,   // Thanh toán tại khách sạn
        Card          // Thanh toán bằng thẻ (online/offline)
    }
}
