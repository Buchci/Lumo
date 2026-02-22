using System.ComponentModel.DataAnnotations;

public class CreateDiaryEntryDto
{
    [Required]
    [MaxLength(200)] 
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTime EntryDate { get; set; }

    [Range(1, 5)] 
    public int MoodRating { get; set; } = 3;

    public bool IsFavorite { get; set; } = false;

    public List<int> TagIds { get; set; } = new();
}