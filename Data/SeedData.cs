using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Seed roles
                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Seed admin user
                var adminUser = await userManager.FindByEmailAsync("admin@hotel.com");
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin@hotel.com",
                        Email = "admin@hotel.com",
                        FirstName = "Admin",
                        LastName = "User",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // Seed rooms
                if (!context.Rooms.Any())
                {
                    context.Rooms.AddRange(
                        new Room
                        {
                            Name = "Deluxe Room",
                            Description = "Spacious room with king-sized bed and city view",
                            Price = 199.99m,
                            Capacity = 2,
                            Size = 45,
                            Type = "Deluxe",
                            ImageUrl = "/images/rooms/deluxe.jpg",
                            IsAvailable = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        },
                        new Room
                        {
                            Name = "Executive Suite",
                            Description = "Luxurious suite with separate living area and balcony",
                            Price = 349.99m,
                            Capacity = 3,
                            Size = 75,
                            Type = "Suite",
                            ImageUrl = "/images/rooms/executive.jpg",
                            IsAvailable = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        },
                        new Room
                        {
                            Name = "Family Room",
                            Description = "Perfect for families with two double beds",
                            Price = 259.99m,
                            Capacity = 4,
                            Size = 55,
                            Type = "Family",
                            ImageUrl = "/images/rooms/family.jpg",
                            IsAvailable = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        },
                        new Room
                        {
                            Name = "Presidential Suite",
                            Description = "Ultimate luxury with private jacuzzi and butler service",
                            Price = 699.99m,
                            Capacity = 2,
                            Size = 120,
                            Type = "Suite",
                            ImageUrl = "/images/rooms/presidential.jpg",
                            IsAvailable = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        },
                        new Room
                        {
                            Name = "Standard Room",
                            Description = "Comfortable room with all essential amenities",
                            Price = 129.99m,
                            Capacity = 2,
                            Size = 35,
                            Type = "Standard",
                            ImageUrl = "/images/rooms/standard.jpg",
                            IsAvailable = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    );

                    await context.SaveChangesAsync();
                }

                // Seed sample user
                var sampleUser = await userManager.FindByEmailAsync("user@example.com");
                if (sampleUser == null)
                {
                    sampleUser = new ApplicationUser
                    {
                        UserName = "user@example.com",
                        Email = "user@example.com",
                        FirstName = "John",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1985, 5, 15),
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(sampleUser, "User123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(sampleUser, "User");
                    }
                }

                // Seed sample bookings
                if (!context.Bookings.Any())
                {
                    var user = await userManager.FindByEmailAsync("user@example.com");
                    var room = await context.Rooms.FirstAsync();

                    if (user != null && room != null)
                    {
                        context.Bookings.AddRange(
                            new Booking
                            {
                                UserId = user.Id,
                                RoomId = room.Id,
                                CheckInDate = DateTime.Today.AddDays(-30),
                                CheckOutDate = DateTime.Today.AddDays(-28),
                                NumberOfGuests = 2,
                                TotalPrice = room.Price * 2,
                                Status = BookingStatus.CheckedOut,
                                CreatedAt = DateTime.UtcNow.AddDays(-35),
                                UpdatedAt = DateTime.UtcNow.AddDays(-28)
                            },
                            new Booking
                            {
                                UserId = user.Id,
                                RoomId = room.Id,
                                CheckInDate = DateTime.Today.AddDays(7),
                                CheckOutDate = DateTime.Today.AddDays(10),
                                NumberOfGuests = 2,
                                TotalPrice = room.Price * 3,
                                Status = BookingStatus.Confirmed,
                                CreatedAt = DateTime.UtcNow.AddDays(-5),
                                UpdatedAt = DateTime.UtcNow.AddDays(-5)
                            }
                        );

                        await context.SaveChangesAsync();
                    }
                }

                // Seed sample reviews
                if (!context.Reviews.Any())
                {
                    var user = await userManager.FindByEmailAsync("user@example.com");
                    var room = await context.Rooms.FirstAsync();

                    if (user != null && room != null)
                    {
                        context.Reviews.Add(
                            new Review
                            {
                                UserId = user.Id,
                                RoomId = room.Id,
                                Rating = 5,
                                Comment = "Excellent room with amazing view! The service was outstanding.",
                                CreatedAt = DateTime.UtcNow.AddDays(-25),
                                UpdatedAt = DateTime.UtcNow.AddDays(-25)
                            }
                        );

                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}