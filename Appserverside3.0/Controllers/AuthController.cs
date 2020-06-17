using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServerSide.Data;

namespace AppServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("register")]
        public async Task<IActionResult> Register(string username,string password)
        {
            username = username.ToLower();

            if (await _repo.UserExists(username))
            {
                return BadRequest("User already Exists");
            }
            else
            {
                var createdUser = await _repo.Register(new Models.User() { Username = username }, password);
                if (createdUser != null)
                {
                    return StatusCode(201);
                }
                else
                {
                    return BadRequest("User Creation Failed.");
                }
            }
        }
    }
}