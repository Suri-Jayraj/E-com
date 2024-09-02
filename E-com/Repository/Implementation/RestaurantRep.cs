using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_com.DAL;
using E_com.Models;
using Microsoft.EntityFrameworkCore;

namespace E_com.Repositories
{
    public class RestaurantRepository : IRestaurantRep
    {
        private readonly MyDbContext _context;

        public RestaurantRepository(MyDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Restaurants>> GetAllRestaurantsAsync()
        {
            return await _context.Restaurants.ToListAsync();
        }

        public async Task<Restaurants> GetRestaurantByIdAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Restaurant ID cannot be null.");
            }

            return await _context.Restaurants.FirstOrDefaultAsync(r => r.RestaurantId == id.Value)
                ?? throw new InvalidOperationException($"Restaurant with ID {id.Value} not found.");
        }



        public async Task AddRestaurantAsync(Restaurants restaurant)
        {
            await _context.Restaurants.AddAsync(restaurant);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        Task<Restaurants> IRestaurantRep.GetRestaurantByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
