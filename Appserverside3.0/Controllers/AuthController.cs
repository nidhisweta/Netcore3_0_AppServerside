using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServerSide.Data;
using Appserverside3._0.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterForDTO userRegisterForDTO)
        {
            userRegisterForDTO.Username = userRegisterForDTO.Username.ToLower();

            if (await _repo.UserExists(userRegisterForDTO.Username))
            {
                return BadRequest("User already Exists");
            }
            else
            {
                var createdUser = await _repo.Register(new Models.User() { Username = userRegisterForDTO.Username }, userRegisterForDTO.Password);
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            var userfromrepo = await _repo.Login(userForLoginDTO.Username, userForLoginDTO.Password);

            if (userfromrepo != null)
                {

                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.Name, userfromrepo.Username));
                claims.AddClaim(new Claim(ClaimTypes.Role, userfromrepo.Role));
             
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("hello"));
                var signature = new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);
                var tokendescription = new SecurityTokenDescriptor()
                {
                    Subject = claims,
                    SigningCredentials=signature,
                    Expires=DateTime.Now.AddDays(1)
                };


                var tokenhandler = new JwtSecurityTokenHandler();
                 var token = tokenhandler.CreateJwtSecurityToken(tokendescription);
                return Ok(new
                {
                    JWTtoken = tokenhandler.WriteToken(token)
                });

                }
            else
                {
                    return Unauthorized();
                }
            

        }
    }
}