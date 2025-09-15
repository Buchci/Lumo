namespace Lumo.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string? ResourceKey { get; set; }

        public string? CustomName { get; set; }

        public bool IsGlobal { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }

}
