using System.ComponentModel.DataAnnotations;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.ViewModels.Booking
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // For creating booking
        public int RoomId { get; set; }

        [Required]
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        public DateTime CheckInDateInput { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [Display(Name = "Check-out Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDateInput { get; set; } = DateTime.Today.AddDays(2);

        [Required]
        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10")]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; } = 1;

        [Display(Name = "Special Requests")]
        [StringLength(500)]
        public string? SpecialRequests { get; set; }
    }

    public class BookingConfirmationViewModel
    {
        public int BookingId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumberOfGuests { get; set; }
        public BookingStatus Status { get; set; }
    }

    public class BookingListViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BookingDetailsViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}