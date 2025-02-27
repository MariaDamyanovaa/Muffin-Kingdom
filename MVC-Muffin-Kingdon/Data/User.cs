using Microsoft.AspNetCore.Identity;

namespace MVC_Muffin_Kingdon.Data
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
    }
}