using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhachSan.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Họ và tên")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "CMND/CCCD")]
        [StringLength(20)]
        public string? IdentityNumber { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}