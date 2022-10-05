using Microsoft.AspNetCore.Identity;

namespace AppTemplate.AuthApi.Database.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
