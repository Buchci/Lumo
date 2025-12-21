using Lumo.Data;
using Lumo.DTOs.Statistics;
using Microsoft.EntityFrameworkCore;


namespace Lumo.Services
{
    public class StatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StatisticsOverviewDto> GetUserStatisticsAsync(string userId)
        {
            var entries = _context.DiaryEntries
                .Where(e => e.UserId == userId);

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
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            var tagUsage = await _context.Tags
                .Where(t => t.IsGlobal || t.UserId == userId)
                .Select(t => new TagUsageDto
                {
                    TagId = t.Id,
                    Name = t.CustomName ?? t.ResourceKey ?? "",
                    Count = t.Entries!.Count(e => e.UserId == userId)
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return new StatisticsOverviewDto
            {
                OverallAverageMood = Math.Round(overallAverage, 2),
                MonthlyAverages = monthly,
                TagUsage = tagUsage
            };
        }
    }
}
