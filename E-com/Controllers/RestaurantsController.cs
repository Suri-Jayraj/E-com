using System.Collections.Generic;
using System.Threading.Tasks;
using E_com.Models;
using E_com.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_com.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRestaurantsList()
        {
            var restaurantData = await _restaurantService.GetAllRestaurantsAsync();
            return Ok(restaurantData);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurants>> GetRestaurant(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return Ok(restaurant);
        }

        [HttpPost]
        [Route("Createresta")]
        public async Task<ActionResult<Restaurants>> PostRestaurant(Restaurants restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _restaurantService.CreateRestaurantAsync(restaurant);
                return Ok(new { message = "Restaurant Added successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while saving the restaurant.");
            }
        }
    }
}
