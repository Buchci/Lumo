using System.ComponentModel.DataAnnotations;
namespace Lumo.DTOs.Tag
{
    public class CreateTagDto
    {
        [MaxLength(20)]
        [Required]
        public string? CustomName { get; set; }

    }
}