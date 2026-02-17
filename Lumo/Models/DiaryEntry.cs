using System.ComponentModel.DataAnnotations;

namespace Lumo.Models
{
    public class DiaryEntry
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

 
        [DataType(DataType.Date)]
        public DateTime EntryDate { get; set; }

        // Daty systemowe
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }

        public bool IsFavorite { get; set; } = false;

        [Range(1, 5)]
        public int MoodRating { get; set; } = 3;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}