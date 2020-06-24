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
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace AppServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _repo;
        private IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _config = config;
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
            try
            {

                var userfromrepo = await _repo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);
                if (userfromrepo != null)
                {

                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userfromrepo.Id.ToString()));
                    claims.AddClaim(new Claim(ClaimTypes.Name, userfromrepo.Username));
                    claims.AddClaim(new Claim(ClaimTypes.Role, userfromrepo.Role));

                    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetSection("Appsettings").GetSection("Token").Value));
                    var key1 = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ProEMLh5e_qnzdNU"));
                    var signature = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                   
                    var tokendescription = new SecurityTokenDescriptor()
                    {
                        Subject = claims,
                        SigningCredentials = signature,
                        Expires = DateTime.Now.AddDays(1),
                      //  EncryptingCredentials=new EncryptingCredentials(key1, SecurityAlgorithms.Aes128KW,
                        //SecurityAlgorithms.Aes128CbcHmacSha256)
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
            catch(Exception ex)
            {
                return BadRequest("erroor" + ex.Message);
            }
        }
    }
}