using E_com.DAL;
using E_com.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_com.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UsersController(MyDbContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpGet]

        public IActionResult Get()
        {
            var Data = _context.Users.ToList();
            if (Data.Count == 0)
            {
                return NotFound("NoData");
            }
            return Ok(Data);
        }

        

    }
}
