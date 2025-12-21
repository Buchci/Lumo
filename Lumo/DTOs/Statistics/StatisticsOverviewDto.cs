namespace Lumo.DTOs.Statistics
{
    public class StatisticsOverviewDto
    {
        public double OverallAverageMood { get; set; }
        public List<MonthlyMoodDto> MonthlyAverages { get; set; } = new();
        public List<TagUsageDto> TagUsage { get; set; } = new();
    }
}
