using AppTemplate.AuthApi.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppTemplate.AuthApi.Database.Contexts;

public class AuthApiDbContext : IdentityDbContext<AppUser>
{
    public AuthApiDbContext(DbContextOptions<AuthApiDbContext> options)
        : base(options) { }
}
