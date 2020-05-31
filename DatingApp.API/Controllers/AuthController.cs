using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{   
    //We will not be using Views in MVC
    //the views will be provided by Angular


    [Route("api/[controller]")]
    [ApiController] 
    public class AuthController : ControllerBase
    {
        readonly IAuthRepository _authRepo;
        readonly IConfiguration _config;
        readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepo, IConfiguration config, IMapper mapper)
        {
            _authRepo = authRepo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegisterDto userRegisterDto)
        {
            //validate request
            userRegisterDto.Username = userRegisterDto.Username.ToLower();
            if( await _authRepo.UserExists(userRegisterDto.Username))
            {
                return BadRequest("Username already exists!");
            }

            var userToCreate = new User{
                Username = userRegisterDto.Username
            };

            var createdUser = await _authRepo.Register(userToCreate, userRegisterDto.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userLoginDto)
        {
            var userFromRepo = await _authRepo
                .Login(userLoginDto.Username.ToLower(), userLoginDto.Password);

            if(userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}