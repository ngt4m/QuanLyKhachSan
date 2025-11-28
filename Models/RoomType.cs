using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhachSan.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
        [Display(Name = "Loại phòng")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Giá cơ bản")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Số khách mặc định tối đa")]
        [Range(1, 10)]
        public int DefaultMaxGuests { get; set; } = 2;

        // Navigation
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
