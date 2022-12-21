using Microsoft.AspNetCore.Identity;

namespace SocialAuth.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
