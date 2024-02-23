using Microsoft.AspNetCore.Identity;

namespace IdentiyApp.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
