using System.ComponentModel.DataAnnotations;

namespace Lumo.DTOs.DiaryEntry
{
    public class UpdateDiaryEntryDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? EntryDate { get; set; }
        public List<int>? TagIds { get; set; }
        [Range(1, 5)]
        public int? MoodRating { get; set; }
        public bool? IsFavorite { get; set; }
    }
}
