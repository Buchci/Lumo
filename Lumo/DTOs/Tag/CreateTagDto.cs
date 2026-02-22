using System.ComponentModel.DataAnnotations;

public class CreateTagDto
{
    [MaxLength(20)]
    public string? ResourceKey { get; set; }

    [MaxLength(20)]
    public string? CustomName { get; set; }

    public bool IsGlobal { get; set; } = false;
}