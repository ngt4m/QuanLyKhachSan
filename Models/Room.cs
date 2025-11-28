using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QuanLyKhachSan.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phòng là bắt buộc")]
        [Display(Name = "Tên phòng")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Số phòng")]
        [StringLength(20)]
        public string? RoomNumber { get; set; }

        [Display(Name = "Tầng")]
        public int? Floor { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [Display(Name = "Mô tả")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Khóa ngoại loại phòng
        [Required]
        [Display(Name = "Loại phòng")]
        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; } = null!;

        [Required(ErrorMessage = "Giá phòng là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phòng phải lớn hơn 0")]
        [Display(Name = "Giá phòng")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số người tối đa là bắt buộc")]
        [Display(Name = "Số người tối đa")]
        [Range(1, 10)]
        public int MaxGuests { get; set; }

        [Display(Name = "Diện tích")]
        [StringLength(20)]
        public string? Size { get; set; }

        [Display(Name = "Tiện nghi")]
        public string? Amenities { get; set; }

        [Display(Name = "Hình ảnh")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Trạng thái")]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [JsonIgnore]
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    public enum RoomStatus
    {
        Available,
        Occupied,
        Maintenance,
        Cleaning
    }
}
