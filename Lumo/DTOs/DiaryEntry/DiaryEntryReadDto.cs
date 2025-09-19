namespace Lumo.DTOs.DiaryEntry
{
    public class DiaryEntryReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public List<string> Tags { get; set; } = new();
        public int MoodRating { get; set; }
        public bool IsFavorite { get; set; }
    }
}
