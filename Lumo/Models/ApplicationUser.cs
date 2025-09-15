using Microsoft.AspNetCore.Identity;

namespace Lumo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nickname { get; set; }
    }
}
