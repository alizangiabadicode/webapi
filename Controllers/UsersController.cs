using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Data;
using datingapp.api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using datingapp.api.Helpers;
using datingapp.api.Models;

namespace datingapp.api.Controllers
{
    [ServiceFilter(typeof(UpdateLastOnline))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository rep;
        private readonly IMapper mapper;
        public UsersController(IDatingRepository rep, IMapper mapper)
        {
            this.mapper = mapper;
            this.rep = rep;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery]PaginationParams p)
        {
            var id = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var user = await rep.GetUser(id);
            if (string.IsNullOrEmpty(p.Gender))
            {
                p.Gender = user.Gender == "male" ? "female" : "male";
            }
            p.UserId = id;
            var users = await rep.GetUsers(p);
            var mappedUsers = mapper.Map<List<UserForLIstDTO>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalPages, users.TotalCount);
            // var id = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            // var user = mappedUsers.FirstOrDefault(i => i.Id == id);
            // if(user.PhotoUrl == null) {
            //     user.PhotoUrl =  "../../assets/defaultuser.png";
            // }
            return Ok(mappedUsers);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await rep.GetUser(id);

            var mappedUser = mapper.Map<UserForDetailDTO>(user);
            // if(mappedUser.PhotoUrl == null)
            //     mappedUser.PhotoUrl = "../../assets/defaultuser.png";

            return Ok(mappedUser);
        }

        [HttpPut("{id}")]//chon update mikonm estefade kardam
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO dto)
        {
            //first i need to authenticate the user
            if (id != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            //if the user is currect one so let's update
            //first get the user with the specifided id
            var usertoupdate = await rep.GetUser(id);
            mapper.Map(dto, usertoupdate);

            if (await rep.SaveAll())
            {
                return NoContent();//204
            }

            //else
            throw new Exception($"user with this id = {id}  cannot update");
        }

        [HttpPost("{likerid}/like/{likeeid}")]
        public async Task<IActionResult> Like(int likerid, int likeeid)
        {
            // first check if it is the real user
            var tokenid = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
            if(likerid != tokenid)
            {
                return Unauthorized();
            }

            // check if the likee exists
            var likee = await rep.GetUser(likeeid);
            if(likee == null) 
            {
                return NotFound();
            }
            // check if the like already exists
            var like = await this.rep.GetLike(likerid, likeeid);
            if(like != null) 
            {
                return BadRequest($"{likee.UserName} is already liked!");
            }

            // so the like request is authorized now insert the like to db
            var newLike = new Like()
            {
                LikerId = likerid ,
                LikeeId = likeeid
            };
            this.rep.Add(newLike);
            if(await this.rep.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("like method failed");
        }
    }
}