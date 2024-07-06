using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stage2.Data.DTOs;
using Stage2.Models;
using Stage2.Services;
using System.ComponentModel.DataAnnotations;

namespace Stage2.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDTO registrationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                firstName = registrationDTO.firstName,
                lastName = registrationDTO.lastName,
                email = registrationDTO.email,
                password = registrationDTO.password,
                phone = registrationDTO.phone
            };

            var result = await _userService.RegisterUserAsync(user);
            if (result == null)
            {
                return StatusCode(400, new
                {
                    status = "Bad request",
                    message = "Registration unsuccessful",
                    statusCode = 400
                });
            }

            var token = _jwtService.GenerateToken(user);

            return StatusCode(201, new
            {
                status = "success",
                message = "Registration successful",
                data = new
                {
                    accessToken = token,
                    user = new
                    {
                        user.UserId,
                        user.firstName,
                        user.lastName,
                        user.email,
                        user.phone
                    }
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.AuthenticateUserAsync(loginDTO.email, loginDTO.password);
            if (user == null)
            {
                return Unauthorized(new
                {
                    status = "Bad request",
                    message = "Authentication failed",
                    statusCode = 401
                });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                status = "success",
                message = "Login successful",
                data = new
                {
                    accessToken = token,
                    user = new
                    {
                        user.UserId,
                        user.firstName,
                        user.lastName,
                        user.email,
                        user.phone
                    }
                }
            });
        }
    }
}/////////////////////////////////////////////////////////////////////////////////////////////////////////////////7738/