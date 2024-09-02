using System.Collections.Generic;
using System.Threading.Tasks;
using E_com.Models;
using E_com.Repositories;

namespace E_com.Services
{
    public class RestaurantService : IRestaurantService
    {
       
        private IRestaurantRep _restaurantRep;

        public RestaurantService(IRestaurantRep restaurantRep)
        {
            _restaurantRep = restaurantRep;
        }

        public async Task<IEnumerable<Restaurants>> GetAllRestaurantsAsync()
        {
            return await _restaurantRep.GetAllRestaurantsAsync();
        }

        public async Task<Restaurants> GetRestaurantByIdAsync(int id)
        {
            //Restaurant = new Restaurants



            return await _restaurantRep.GetRestaurantByIdAsync(id);
        }

        public async Task CreateRestaurantAsync(Restaurants restaurant)
        {   
            await _restaurantRep.AddRestaurantAsync(restaurant);
            await _restaurantRep.SaveChangesAsync();
        }
    }
}
