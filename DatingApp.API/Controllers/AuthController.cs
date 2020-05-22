using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{   
    //We will not be using Views in MVC
    //the views will be provided by Angular


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            //validate request
            username = username.ToLower();
            if( await _authRepo.UserExists(username))
            {
                return BadRequest("Username already exists!");
            }

            var userToCreate = new User{
                Username = username
            };

            var createdUser = await _authRepo.Register(userToCreate, password);
            return StatusCode(201);
        }
    }
}