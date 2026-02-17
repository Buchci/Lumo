using Lumo.Data;
using Lumo.DTOs.Statistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


namespace Lumo.Services
{
    public interface IStatisticsService
    {
        Task<StatisticsOverviewDto> GetUserStatisticsAsync(string userId);
    }
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer _localizer;
        public StatisticsService(ApplicationDbContext context, IStringLocalizerFactory factory)
        {
            _context = context;
            var assemblyName = typeof(Program).Assembly.GetName().Name!;
            _localizer = factory.Create("Tags", assemblyName);
        }

        public async Task<StatisticsOverviewDto> GetUserStatisticsAsync(string userId)
        {
            var entries = _context.DiaryEntries.Where(e => e.UserId == userId);

            var overallAverage = await entries.AnyAsync()
                ? await entries.AverageAsync(e => e.MoodRating)
                : 0;

            var monthly = await entries
                .GroupBy(e => new { e.EntryDate.Year, e.EntryDate.Month })
                .Select(g => new MonthlyMoodDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    AverageMood = Math.Round(g.Average(e => e.MoodRating), 2)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var rawTagUsage = await _context.Tags
                .Where(t => t.IsGlobal || t.UserId == userId)
                .Select(t => new
                {
                    t.CustomName,
                    t.ResourceKey,
                    t.IsGlobal,
                    Count = t.Entries!.Count(e => e.UserId == userId)
                })
                .Where(x => x.Count > 0) // Interesują nas tylko użyte tagi
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var translatedTagUsage = rawTagUsage.Select(t => new TagUsageDto
            {
                Name = t.CustomName
                       ?? (t.IsGlobal && !string.IsNullOrEmpty(t.ResourceKey)
                           ? _localizer[t.ResourceKey].Value
                           : t.ResourceKey ?? ""),
                Count = t.Count
            }).ToList();

            return new StatisticsOverviewDto
            {
                OverallAverageMood = Math.Round(overallAverage, 2),
                MonthlyAverages = monthly,
                TagUsage = translatedTagUsage
            };
        }
    }
}