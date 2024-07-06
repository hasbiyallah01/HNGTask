using Microsoft.AspNetCore.Mvc;
using Moq;
using Stage2.Controller;
using Stage2.Data.DTOs;
using Stage2.Models;
using Stage2.Services;
using System.Threading.Tasks;
using Xunit;
namespace Stage2.tests
{
    public class auth
    {
		private readonly Mock<UserService> _mockUserService;
		private readonly Mock<JwtService> _mockJwtService;
		private readonly AuthController _authController;

		public AuthControllerTests()
		{
			_mockUserService = new Mock<UserService>();
			_mockJwtService = new Mock<JwtService>();
			_authController = new AuthController(_mockUserService.Object, _mockJwtService.Object);
		}

		[Fact]
		public async Task Register_ShouldReturn201_WhenRegistrationIsSuccessful()
		{
			var registrationDTO = new UserRegistrationDTO
			{
				firstName = "John",
				lastName = "Doe",
				email = "john.doe@example.com",
				password = "Password123!",
				phone = "1234567890"
			};

			var user = new User
			{
				UserId = "1",
				firstName = registrationDTO.firstName,
				lastName = registrationDTO.lastName,
				email = registrationDTO.email,
				password = registrationDTO.password,
				phone = registrationDTO.phone
			};

			_mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<User>())).ReturnsAsync(user);
			_mockJwtService.Setup(service => service.GenerateToken(It.IsAny<User>())).Returns("generated-token");

			var result = await _authController.Register(registrationDTO) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(201, result.StatusCode);
			Assert.Equal("success", ((dynamic)result.Value).status);
			Assert.Equal("generated-token", ((dynamic)result.Value).data.accessToken);
		}

		[Fact]
		public async Task Register_ShouldReturn400_WhenRegistrationFails()
		{
			var registrationDTO = new UserRegistrationDTO
			{
				firstName = "John",
				lastName = "Doe",
				email = "john.doe@example.com",
				password = "Password123!",
				phone = "1234567890"
			};

			_mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<User>())).ReturnsAsync((User)null);

			var result = await _authController.Register(registrationDTO) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(400, result.StatusCode);
			Assert.Equal("Bad request", ((dynamic)result.Value).status);
		}

		[Fact]
		public async Task Login_ShouldReturn200_WhenLoginIsSuccessful()
		{
			var loginDTO = new UserLoginDTO
			{
				email = "john.doe@example.com",
				password = "Password123!"
			};

			var user = new User
			{
				UserId = "1",
				firstName = "John",
				lastName = "Doe",
				email = loginDTO.email,
				password = loginDTO.password,
				phone = "1234567890"
			};

			_mockUserService.Setup(service => service.AuthenticateUserAsync(loginDTO.email, loginDTO.password)).ReturnsAsync(user);
			_mockJwtService.Setup(service => service.GenerateToken(It.IsAny<User>())).Returns("generated-token");

			var result = await _authController.Login(loginDTO) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(200, result.StatusCode);
			Assert.Equal("success", ((dynamic)result.Value).status);
			Assert.Equal("generated-token", ((dynamic)result.Value).data.accessToken);
		}

		[Fact]
		public async Task Login_ShouldReturn401_WhenLoginFails()
		{
			var loginDTO = new UserLoginDTO
			{
				email = "john.doe@example.com",
				password = "Password123!"
			};

			_mockUserService.Setup(service => service.AuthenticateUserAsync(loginDTO.email, loginDTO.password)).ReturnsAsync((User)null);

			var result = await _authController.Login(loginDTO) as ObjectResult;

			Assert.NotNull(result);
			Assert.Equal(401, result.StatusCode);
			Assert.Equal("Bad request", ((dynamic)result.Value).status);
		}
	}
}
