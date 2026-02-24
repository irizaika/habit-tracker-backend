using Microsoft.AspNetCore.Identity;

namespace Authentication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
