using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Data;
using QuanLyKhachSan.sln.Models;
using QuanLyKhachSan.ViewModels.Room;

namespace QuanLyKhachSan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var featuredRooms = await _context.Rooms
                .Where(r => r.IsAvailable)
                .OrderByDescending(r => r.Id)
                .Take(3)
                .Select(r => new RoomCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description.Length > 100 ? r.Description.Substring(0, 100) + "..." : r.Description,
                    Price = r.Price,
                    Type = r.Type,
                    ImageUrl = r.ImageUrl,
                    IsAvailable = r.IsAvailable,
                    AverageRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0,
                    ReviewCount = r.Reviews.Count
                })
                .ToListAsync();

            return View(featuredRooms);
        }

        public async Task<IActionResult> Rooms(string searchTerm, string roomType, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Rooms.Where(r => r.IsAvailable).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Name.Contains(searchTerm) || r.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(roomType))
            {
                query = query.Where(r => r.Type == roomType);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(r => r.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(r => r.Price <= maxPrice.Value);
            }

            var rooms = await query
                .Select(r => new RoomCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description.Length > 100 ? r.Description.Substring(0, 100) + "..." : r.Description,
                    Price = r.Price,
                    Type = r.Type,
                    ImageUrl = r.ImageUrl,
                    IsAvailable = r.IsAvailable,
                    AverageRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0,
                    ReviewCount = r.Reviews.Count
                })
                .ToListAsync();

            var viewModel = new RoomListViewModel
            {
                Rooms = rooms,
                SearchTerm = searchTerm,
                RoomType = roomType,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}