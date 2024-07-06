using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stage2.Services;
using System.Security.Claims;

namespace Stage2.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByIdAsync(id, currentUserId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                status = "success",
                message = "User retrieved successfully",
                data = new
                {
                    user.UserId,
                    user.firstName,
                    user.lastName,
                    user.email,
                    user.phone
                }
            });
        }
    }
}
