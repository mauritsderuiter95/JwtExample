using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JwtExample.Models;
using JwtExample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserService _userService;

        public UsersController(ILogger<UsersController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IEnumerable<User> Get()
        {
            return _userService.Get();
        }

        [HttpGet("{userId}", Name = "GetOne")]
        public User GetOne([FromRoute]string userId)
        {
            return _userService.Get(userId);
        }

        [HttpPost]
        public IActionResult CreateOne([FromBody]User user)
        {
            user = _userService.Create(user);
            return CreatedAtRoute("GetOne", new { userId = user.Id }, user);
        }

        [HttpPut("{userId}")]
        public User UpdateOne([FromRoute]string userId, [FromBody]User user)
        {
            _userService.Update(userId, user);
            return user;
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteOne([FromRoute]string userId)
        {
            _userService.Remove(userId);
            return NoContent();
        }
    }
}
