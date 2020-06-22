using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppServerSide.Data;
using Microsoft.EntityFrameworkCore;
using AppServerSide.Models;
using Microsoft.AspNetCore.Authorization;

namespace AppServerSide.Controllers
{   
    [Authorize(Roles = "Admin,User")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private DataContext _context;
       public  ValuesController(DataContext context)
        {
            _context = context;
        }
        // GET api/values

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _context.Values.ToListAsync();
            return Ok(result);
            }

        // GET api/values/5
        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result =await  _context.Values.Where(v => v.ID == id).ToListAsync();
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
