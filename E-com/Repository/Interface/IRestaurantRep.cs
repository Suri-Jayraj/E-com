


using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using E_com.Models;

namespace E_com.Repositories
{
    public interface IRestaurantRep
    {
        Task<IEnumerable<Restaurants>> GetAllRestaurantsAsync();
        Task<Restaurants> GetRestaurantByIdAsync(int id);
        Task AddRestaurantAsync(Restaurants restaurant);
        Task SaveChangesAsync();
    }
}
