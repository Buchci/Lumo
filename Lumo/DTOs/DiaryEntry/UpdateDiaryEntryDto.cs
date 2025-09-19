namespace Lumo.DTOs.DiaryEntry
{
    public class UpdateDiaryEntryDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? EntryDate { get; set; }
        public List<int>? TagIds { get; set; }
        public int? MoodRating { get; set; }
        public bool? IsFavorite { get; set; }
    }
}
