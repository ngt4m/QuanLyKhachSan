namespace QuanLyKhachSan.ViewModels.Payment
{
    public class PaymentViewModel
    {
        public int BookingId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumberOfGuests { get; set; }
    }

    public class PaymentHistoryViewModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public Models.PaymentStatus Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
    }
}