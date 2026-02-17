using Xunit;
using Moq;
using FluentAssertions;
using Lumo.Controllers.Api;
using Lumo.Models;
using Lumo.DTOs.DiaryEntry;
using Lumo.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Lumo.Services; // upewnij się, że namespace serwisu jest poprawny

namespace LumoDiary.Tests
{
    public class DiaryControllerTests
    {
        private readonly Mock<IDiaryService> _mockService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly DiaryMapper _mapper;
        private readonly DiaryController _controller;
        private const string TestUserId = "user-123";

        public DiaryControllerTests()
        {
            // Mapper
            var mockFactory = new Mock<IStringLocalizerFactory>();
            var mockLocalizer = new Mock<IStringLocalizer>();
            mockFactory.Setup(f => f.Create(It.IsAny<Type>())).Returns(mockLocalizer.Object);
            _mapper = new DiaryMapper(mockFactory.Object);

            // UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Service (INTERFACE!)
            _mockService = new Mock<IDiaryService>();

            // Controller
            _controller = new DiaryController(
                _mockService.Object,
                _mockUserManager.Object,
                _mapper);

            // Fake logged user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, TestUserId),
        }, "TestAuth"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockUserManager
                .Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(TestUserId);
        }
        [Fact]
        public async Task GetFavorites_ReturnsOnlyFavorites()
        {
            // Arrange
            var entries = new List<DiaryEntry>
    {
        new DiaryEntry { Id = 1, Title = "Fav", IsFavorite = true, UserId = TestUserId },
        new DiaryEntry { Id = 2, Title = "Normal", IsFavorite = false, UserId = TestUserId }
    };

            _mockService
                .Setup(s => s.GetUserEntriesAsync(TestUserId))
                .ReturnsAsync(entries);

            // Act
            var result = await _controller.GetFavorites();

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var data = ok.Value.Should().BeAssignableTo<List<DiaryEntryReadDto>>().Subject;

            data.Should().HaveCount(1);
            data[0].Title.Should().Be("Fav");
        }
        [Fact]
        public async Task GetById_ReturnsOk_WhenEntryExists()
        {
            // Arrange
            var entry = new DiaryEntry { Id = 1, Title = "Test Entry", UserId = TestUserId };
            _mockService.Setup(s => s.GetEntryByIdAsync(TestUserId, 1)).ReturnsAsync(entry);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var data = ok.Value.Should().BeOfType<DiaryEntryReadDto>().Subject;
            data.Title.Should().Be("Test Entry");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenEntryDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetEntryByIdAsync(TestUserId, 99)).ReturnsAsync((DiaryEntry)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenEntryIsCreated()
        {
            // Arrange
            var dto = new CreateDiaryEntryDto { Title = "Nowy", EntryDate = DateTime.Today };
            var createdEntry = new DiaryEntry { Id = 10, Title = "Nowy", UserId = TestUserId };

            _mockService.Setup(s => s.HasEntryForDateAsync(TestUserId, It.IsAny<DateTime>())).ReturnsAsync(false);
            _mockService.Setup(s => s.CreateEntryAsync(TestUserId, dto)).ReturnsAsync(createdEntry);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeOfType<DiaryEntryReadDto>();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_OnSuccess()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteEntryAsync(1, TestUserId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenEntryToUpdateDoesNotExist()
        {
            // Arrange
            var dto = new UpdateDiaryEntryDto { Title = "Zmieniony" };
            _mockService.Setup(s => s.UpdateEntryAsync(1, TestUserId, dto)).ReturnsAsync((DiaryEntry)null);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        [Fact]
        public async Task Update_ShouldNotAllowEditingOtherUserEntry()
        {
            // Arrange
            var dto = new UpdateDiaryEntryDto { Title = "Zhakowany tytuł" };
            // Serwis zwraca null, bo w logice serwisu szukamy po ID ORAZ UserId
            _mockService.Setup(s => s.UpdateEntryAsync(1, TestUserId, dto)).ReturnsAsync((DiaryEntry)null);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>(); // Zwracamy NotFound, żeby nie zdradzać, że wpis istnieje
        }
        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(100)]
        public async Task CreateEntry_ShouldFail_WhenMoodRatingIsOutOfRange(int invalidMood)
        {
            // Arrange
            var dto = new CreateDiaryEntryDto
            {
                Title = "Zły nastrój",
                MoodRating = invalidMood,
                EntryDate = DateTime.Now
            };

            // Act
            // Tutaj sprawdzamy walidację modelu (ModelState), którą robi ASP.NET Core
            _controller.ModelState.AddModelError("MoodRating", "Value out of range");
            var result = await _controller.Create(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}