using System.ComponentModel.DataAnnotations;

namespace QuanLyKhachSan.ViewModels.Admin
{
    public class RoomCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }

        [Required]
        [Range(1, 500)]
        public int Size { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        //[Url]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;
    }
}