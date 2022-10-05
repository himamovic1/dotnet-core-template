using AppTemplate.AuthApi.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AppTemplate.AuthApi.Database.Contexts;

public class AuthApiDbContext : DbContext
{
    public AuthApiDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<AppUser> AppUsers { get; set; }
}
