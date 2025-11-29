namespace QuanLyKhachSan.ViewModels.Room
{
    public class RoomListViewModel
    {
        public List<RoomCardViewModel> Rooms { get; set; }
        public string SearchTerm { get; set; }
        public string RoomType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public class RoomCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}