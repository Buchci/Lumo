using FluentAssertions;
using Lumo.Data;
using Lumo.DTOs.Statistics;
using Lumo.Models;
using Lumo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Moq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Lumo.Controllers.Api;
namespace LumoDiary.Tests
{
    public class StatisticsControllerTests
    {
        private readonly Mock<IStatisticsService> _mockService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly StatisticsController _controller;
        private const string TestUserId = "user-123";

        // 1. Dodaj pole dla opcji bazy danych
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

        public StatisticsControllerTests()
        {
            // 2. Zainicjalizuj opcje bazy In-Memory
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockService = new Mock<IStatisticsService>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            _controller = new StatisticsController(_mockService.Object, _mockUserManager.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, TestUserId) }));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(TestUserId);
        }

        // --- Test Kontrolera (używa Mocka) ---
        [Fact]
        public async Task Get_ReturnsOk_WithStats()
        {
            _mockService.Setup(s => s.GetUserStatisticsAsync(TestUserId))
                .ReturnsAsync(new StatisticsOverviewDto { OverallAverageMood = 4.5 });

            var result = await _controller.Get();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var data = ok.Value.Should().BeOfType<StatisticsOverviewDto>().Subject;
            data.OverallAverageMood.Should().Be(4.5);
        }

        // --- Test Serwisu (używa prawdziwej bazy In-Memory) ---
        [Fact]
        public async Task GetUserStatisticsAsync_ShouldGroupMonthlyAveragesCorrectly()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbOptions);
            var userId = "user-stats";

            context.DiaryEntries.AddRange(
                new DiaryEntry { UserId = userId, MoodRating = 5, EntryDate = new DateTime(2025, 1, 1) },
                new DiaryEntry { UserId = userId, MoodRating = 3, EntryDate = new DateTime(2025, 1, 15) },
                new DiaryEntry { UserId = userId, MoodRating = 2, EntryDate = new DateTime(2025, 2, 10) }
            );
            await context.SaveChangesAsync();

            var mockFactory = new Mock<IStringLocalizerFactory>();
            var mockLocalizer = new Mock<IStringLocalizer>();
            mockFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(mockLocalizer.Object);

            // Używamy prawdziwego serwisu, a nie mocka, żeby przetestować logikę matematyczną
            var service = new StatisticsService(context, mockFactory.Object);

            // Act
            var result = await service.GetUserStatisticsAsync(userId);

            // Assert
            result.MonthlyAverages.Should().HaveCount(2);
            result.MonthlyAverages.First(m => m.Month == 1).AverageMood.Should().Be(4.0);
            result.MonthlyAverages.First(m => m.Month == 2).AverageMood.Should().Be(2.0);
        }
    }
}