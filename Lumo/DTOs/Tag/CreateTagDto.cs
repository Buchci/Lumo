namespace Lumo.DTOs.Tag
{
    public class CreateTagDto
    {
        public string? ResourceKey { get; set; } = null;
        public string? CustomName { get; set; }
        public bool IsGlobal { get; set; } = false;
    }
}
