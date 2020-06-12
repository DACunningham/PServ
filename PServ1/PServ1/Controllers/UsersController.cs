using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PServ1.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PServ1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        //private readonly IRewardRepository _rewardRepository;
        private readonly string _userCountId = "UserCount";

        public UsersController(IUserRepository userRepository /*, IRewardRepository rewardRepository*/)
        {
            _userRepository = userRepository;
            //_rewardRepository = rewardRepository;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public IActionResult Get()
        {
            var userCount = _userRepository.GetUserCount(_userCountId);
            return Ok(userCount);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
