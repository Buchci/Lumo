namespace Lumo.DTOs.DiaryEntry
{
    public class CreateDiaryEntryDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public List<int> TagIds { get; set; } = new();
        public int MoodRating { get; set; } = 3;
        public bool IsFavorite { get; set; } = false;
    }
}
