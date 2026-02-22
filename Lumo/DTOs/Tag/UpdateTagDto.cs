using System.ComponentModel.DataAnnotations;

namespace Lumo.DTOs.Tag
{
    public class UpdateTagDto
    {
        [MaxLength(20)]
        public string? CustomName { get; set; } 
    }
}
