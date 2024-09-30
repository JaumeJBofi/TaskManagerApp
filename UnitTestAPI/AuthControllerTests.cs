using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagerApi.Controllers;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos.Authentication;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Utilities;
using System.Linq;
using TaskManagerApi.Services;

namespace TaskManagerApi.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IConfig> _configMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IJwtTokenService> _jwtTokenServiceMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private Mock<IAntiforgery> _antiforgeryMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _configMock = new Mock<IConfig>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _antiforgeryMock = new Mock<IAntiforgery>();
            _controller = new AuthController(
                _configMock.Object,
                _userRepositoryMock.Object,
                _jwtTokenServiceMock.Object,
                _passwordServiceMock.Object,
                _antiforgeryMock.Object
            );

            // Mock the HttpContext and User
            var httpContext = new DefaultHttpContext();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId")
            }, "mock"));
            httpContext.User = user;
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        } 
    

        [Test]
        public async Task SignIn_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var signInDto = new SignInDto();
            _controller.ModelState.AddModelError("Password", "The Password field is required.");

            // Act
            var result = await _controller.SignIn(signInDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task SignIn_UserExists_ReturnsUnauthorized()
        {
            // Arrange
            var signInDto = new SignInDto
            {
                UserName = "existinguser",
                Email = "test@example.com",
                Password = "testpassword123"
            };
            _userRepositoryMock.Setup(x => x.GetByUserNameAsync(signInDto.UserName)).ReturnsAsync(new User { UserName = signInDto.UserName });

            // Act
            var result = await _controller.SignIn(signInDto);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = (UnauthorizedObjectResult)result;

            Assert.IsNotNull(unauthorizedResult.Value);
            Assert.That(unauthorizedResult.Value.GetType().GetProperty("message").GetValue(unauthorizedResult.Value), Is.EqualTo("Username already exist"));
        }

        [Test]
        public async Task SignIn_WeakPassword_ReturnsUnauthorized()
        {
            // Arrange
            var signInDto = new SignInDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "weakpassword"
            };
            _passwordServiceMock.Setup(x => x.GetPasswordSecurityRating(signInDto.Password)).Returns(2);

            // Act
            var result = await _controller.SignIn(signInDto);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = (UnauthorizedObjectResult)result;

            Assert.IsNotNull(unauthorizedResult.Value);
            Assert.AreEqual("Password not secure enough", unauthorizedResult.Value.GetType().GetProperty("message").GetValue(unauthorizedResult.Value));
        }
     

        [Test]
        public async Task Login_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto();
            _controller.ModelState.AddModelError("Password", "The Password field is required.");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "testuser",
                Password = "wrongpassword"
            };
            var user = new User { UserName = loginDto.UserName, PasswordHash = "hashedPassword" };
            _userRepositoryMock.Setup(x => x.GetByUserNameAsync(loginDto.UserName)).ReturnsAsync(user);
            _passwordServiceMock.Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(false);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedResult = (UnauthorizedObjectResult)result;

            Assert.IsNotNull(unauthorizedResult.Value);
            Assert.AreEqual("Invalid username or password", unauthorizedResult.Value.GetType().GetProperty("message").GetValue(unauthorizedResult.Value));
        }         

        [Test]
        public async Task Logout_InvalidModel_ReturnsBadRequest()
        {
            // Arrange        
            _controller.ModelState.AddModelError("UserName", "The User Name field is required.");

            // Act
            var result = await _controller.Logout();

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}