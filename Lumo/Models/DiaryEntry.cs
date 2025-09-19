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

        // Data wpisu (wybierana przez użytkownika)
        [DataType(DataType.Date)]
        public DateTime EntryDate { get; set; }

        // Daty systemowe
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }

        public bool IsFavorite { get; set; } = false;

        // Ocena nastroju w skali 1–5
        [Range(1, 5)]
        public int MoodRating { get; set; } = 3;

        // Autor
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        // Relacja wiele-do-wielu z Tagami
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}