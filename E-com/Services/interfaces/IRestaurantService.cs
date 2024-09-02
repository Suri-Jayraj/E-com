using System.Collections.Generic;
using System.Threading.Tasks;
using E_com.Models;

namespace E_com.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurants>> GetAllRestaurantsAsync();
        Task<Restaurants> GetRestaurantByIdAsync(int id);
        Task CreateRestaurantAsync(Restaurants restaurant);
    }
}
