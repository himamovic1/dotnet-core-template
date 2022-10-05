using AppTemplate.AuthApi.Database.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace AppTemplate.AuthApi.Extensions.Startup;

/// <summary>
/// Extension methods to simplify configuring Identity Server
/// </summary>
public static class IdentityServerExtensions
{
    public static IServiceCollection AddIdentityServerConfig(this IServiceCollection services, IConfiguration _config)
    {
        string identityConnectionString = _config.GetConnectionString("IdentityDbContext");

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
        })
         .AddEntityFrameworkStores<IdentityDbContext>()
         .AddDefaultTokenProviders();

        services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<AppUser>()
                //Configuration Store: clients and resources
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = db =>
                    db.UseSqlServer(identityConnectionString,
                        sql => sql.MigrationsAssembly(InternalAssemblies.Database));
                })
                //Operational Store: tokens, codes etc.
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = db =>
                    db.UseSqlServer(identityConnectionString,
                        sql => sql.MigrationsAssembly(InternalAssemblies.Database));
                })
                .AddProfileService<IdentityProfileService>(); // custom claims 

        //Cache Discovery document HttpClient
        services.AddSingleton<IDiscoveryCache>(r =>
        {
            var factory = r.GetRequiredService<IHttpClientFactory>();
            return new DiscoveryCache(_config["AuthApiUrl"], () => factory.CreateClient());
        });

        return services;
    }
}
