using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JwtExample.Models;
using JwtExample.Services;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly ILogger<TokensController> _logger;
        private readonly UserService _userService;

        public TokensController(ILogger<TokensController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("accesstoken", Name = "login")]
        public IActionResult Login([FromBody]Authentication auth)
        {
            try {
                return  Ok(_userService.Login(auth));
            } 
            catch(Exception e) {
                return BadRequest(e);
            }
        }

        [Authorize(AuthenticationSchemes = "refresh")]
        [HttpPut("accesstoken", Name = "refresh")]
        public IActionResult Refresh()
        {
            Claim refreshtoken = User.Claims.FirstOrDefault(x => x.Type == "refresh");
            Claim username = User.Claims.FirstOrDefault(x => x.Type == "username");

            try
            {
                return Ok(_userService.Refresh(username, refreshtoken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
