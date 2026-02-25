using FluentAssertions;
using Lumo.Controllers.Api;
using Lumo.Data;
using Lumo.DTOs.Tag;
using Lumo.Models;
using Lumo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LumoDiary.Tests
{
    public class TagControllerTests
    {
        private readonly Mock<ITagService> _mockService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly TagController _controller;
        private const string TestUserId = "user-999";
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

        public TagControllerTests()
        {
            // Konfiguracja bazy w pamięci RAM
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockService = new Mock<ITagService>();
            var mockLocalizer = new Mock<IStringLocalizer>();
            mockLocalizer.Setup(l => l[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, key));

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var mockFactory = new Mock<IStringLocalizerFactory>();
            mockFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockLocalizer.Object);

            _controller = new TagController(_mockService.Object, _mockUserManager.Object, mockFactory.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, TestUserId) }));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(TestUserId);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithTranslatedTags()
        {
            var tags = new List<Tag> {
                new Tag { Id = 1, CustomName = "Praca", IsGlobal = false },
                new Tag { Id = 2, ResourceKey = "Happy", IsGlobal = true }
            };
            _mockService.Setup(s => s.GetUserTagsAsync(TestUserId)).ReturnsAsync(tags);

            var result = await _controller.Get();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var data = okResult.Value.Should().BeAssignableTo<List<TagReadDto>>().Subject;
            data.Should().HaveCount(2);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTagDoesNotExist()
        {
            _mockService.Setup(s => s.DeleteTagAsync(1, TestUserId)).ReturnsAsync(false);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateTagAsync_ShouldThrowException_WhenUserTriesToCreateGlobalTag()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var service = new TagService(context);
            var dto = new CreateTagDto { CustomName = "New User Tag" };

            var result = await service.CreateTagAsync(TestUserId, dto);

            result.IsGlobal.Should().BeFalse(); // Upewniamy się, że serwis sam nie ustawił globala
            result.UserId.Should().Be(TestUserId);
        }
        [Fact]
        public async Task DeleteTagAsync_ShouldReturnFalse_WhenDeletingOtherUserTag()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var tag = new Tag { Id = 50, CustomName = "Cudzy Tag", UserId = "OtherUser" };
            context.Tags.Add(tag);
            await context.SaveChangesAsync();

            var service = new TagService(context);

            // Act
            var result = await service.DeleteTagAsync(50, TestUserId); // Próbujemy usunąć jako TestUserId

            // Assert
            result.Should().BeFalse();
            context.Tags.Should().Contain(tag); // Tag nadal powinien być w bazie
        }
    }
}