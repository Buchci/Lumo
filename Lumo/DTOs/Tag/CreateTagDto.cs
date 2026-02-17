namespace Lumo.DTOs.Tag
{
    public class CreateTagDto
    {
        public string? ResourceKey { get; set; } = null;   // np. klucz dla tagów systemowych
        public string? CustomName { get; set; }    // nazwa tagu wpisana przez użytkownika
        public bool IsGlobal { get; set; } = false; // czy tag jest globalny
    }
}
