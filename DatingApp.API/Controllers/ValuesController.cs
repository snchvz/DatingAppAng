using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    //We will not be using Views in MVC
    //the views will be provided by Angular

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase  //ControllerBase -> NO view support
    {
        readonly DataContext _db;

        public ValuesController(DataContext db)
        {
            _db = db;    
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()  //allows us to return HTTP responses to the client
        {
            var values = await _db.Values.ToListAsync();
            return Ok(values);           
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _db.Values.FirstOrDefaultAsync(t => t.id == id);
            return Ok(value);

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
