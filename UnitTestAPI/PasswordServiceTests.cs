using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerApi.Models;
using TaskManagerApi.Services;
using TaskManagerApi.Utilities;

namespace UnitTests.API
{

    [TestFixture]
    public class PasswordServiceTests
    {
        private PasswordService _passwordService;

        [SetUp]
        public void Setup()
        {
            _passwordService = new PasswordService();
        }

        [Test]
        public void GetPasswordSecurityRating_WeakPassword_ReturnsLowRating()
        {
            // Arrange
            var password = "password";

            // Act
            var rating = _passwordService.GetPasswordSecurityRating(password);

            // Assert
            Assert.LessOrEqual(rating, 1);
        }

        [Test]
        public void GetPasswordSecurityRating_MediumPassword_ReturnsMediumRating()
        {
            // Arrange
            var password = "Pdsdsord123*@";

            // Act
            var rating = _passwordService.GetPasswordSecurityRating(password);

            // Assert
            Assert.GreaterOrEqual(rating, 3);
            Assert.LessOrEqual(rating, 4);
        }

        [Test]
        public void GetPasswordSecurityRating_StrongPassword_ReturnsHighRating()
        {
            // Arrange
            var password = "P@$$wOrd123!";

            // Act
            var rating = _passwordService.GetPasswordSecurityRating(password);

            // Assert
            Assert.AreEqual(2, rating);
        }

        [Test]
        public void VerifyPassword_ValidPassword_ReturnsTrue()
        {
            // Arrange
            var password = "testpassword";
            var passwordHash = _passwordService.HashPassword(password);

            // Act
            var isValid = _passwordService.VerifyPassword(password, passwordHash);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void VerifyPassword_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var password = "testpassword";
            var passwordHash = _passwordService.HashPassword(password);

            // Act
            var isValid = _passwordService.VerifyPassword("wrongpassword", passwordHash);

            // Assert
            Assert.IsFalse(isValid);
        }
    }
}
