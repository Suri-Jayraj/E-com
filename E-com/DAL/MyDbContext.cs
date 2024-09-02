using E_com.Models;
using Microsoft.EntityFrameworkCore;

namespace E_com.DAL
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Restaurants> Restaurants { get; set;}

        public DbSet<RestaurantViewModel> Restaurants_new { get; set; }
    }
}



