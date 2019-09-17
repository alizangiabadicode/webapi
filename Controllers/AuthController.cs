using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Data;
using datingapp.api.Dtos;
using datingapp.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace datingapp.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository rep;
        private readonly IConfiguration config;
        private readonly IMapper mapper;
        public AuthController(IAuthRepository rep, IConfiguration config, IMapper mapper)
        {
            this.mapper = mapper;
            this.config = config;
            this.rep = rep;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userdto)
        {
            userdto.UserName = userdto.UserName.ToLower();
            if (await rep.UserExists(userdto.UserName))
            {
                return BadRequest("the user already exists");
            }

            User user = mapper.Map<User>(userdto);
            user = await rep.Register(user, userdto.Password);
            var retuser = mapper.Map<UserForDetailDTO>(user);
            return CreatedAtRoute("GetUser", new {id = user.Id}, retuser);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userdto)
        {
            //throw new Exception("this is the exception");

            var user = await rep.Login(userdto.UserName, userdto.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new[]{
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,userdto.UserName)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };

            var tokenhandler = new JwtSecurityTokenHandler();
            var token = tokenhandler.CreateToken(tokenDiscriptor);
            var mainPhotoUrl = user.Photos.FirstOrDefault(e => e.IsMain);
            return Ok(new
            {
                token = tokenhandler.WriteToken(token),
                mainphotourl = mainPhotoUrl?.Url
            });

        }

    }
}