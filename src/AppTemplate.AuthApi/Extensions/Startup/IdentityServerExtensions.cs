using AppTemplate.AuthApi.Configuration;
using AppTemplate.AuthApi.Database.Contexts;
using AppTemplate.AuthApi.Database.Models;
using AppTemplate.AuthApi.Services.Identity;
using IdentityModel.Client;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AppTemplate.AuthApi.Extensions.Startup;

/// <summary>
/// Extension methods to simplify configuring Identity Server
/// </summary>
public static class IdentityServerExtensions
{
    public static IServiceCollection AddIdentityServerConfig(this IServiceCollection services, IConfiguration _config)
    {
        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        string identityConnectionString = _config.GetConnectionString("AuthApiDbContext");

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<AuthApiDbContext>()
        .AddDefaultTokenProviders();

        services
            .AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddAspNetIdentity<AppUser>()
            //Configuration Store: clients and resources
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(identityConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            //Operational Store: tokens, codes etc.
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(identityConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
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


    public async static Task UseIdentityServerDataAsync(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

        await serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.MigrateAsync();
        await serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

        var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        await configurationContext.Database.MigrateAsync();

        // Get configuration from db
        var dbClients = await configurationContext.Clients.ToListAsync();
        var dbApiScopes = await configurationContext.ApiScopes.ToListAsync();
        var dbApiResources = await configurationContext.ApiResources.ToListAsync();
        var dbIdentityResources = await configurationContext.IdentityResources.ToListAsync();

        // check if some configuration is missing in database if yes insert into database
        foreach (var client in IdentityConfiguration.GetClients(configuration))
        {
            if (!dbClients.Any(x => x.ClientId == client.ClientId))
                configurationContext.Clients.Add(client.ToEntity());
        }

        foreach (var scope in IdentityConfiguration.GetApiScopes())
        {
            if (!dbApiScopes.Any(x => x.Name == scope.Name))
                configurationContext.ApiScopes.Add(scope.ToEntity());
        }

        foreach (var apiResource in IdentityConfiguration.GetResourceApis())
        {
            if (!dbApiResources.Any(x => x.Name == apiResource.Name))
                configurationContext.ApiResources.Add(apiResource.ToEntity());
        }

        foreach (var identityResource in IdentityConfiguration.GetIdentityResources())
        {
            if (!dbIdentityResources.Any(x => x.Name == identityResource.Name))
                configurationContext.IdentityResources.Add(identityResource.ToEntity());
        }

        await configurationContext.SaveChangesAsync();

        // seed the Identity Roles (define here which roles application will have)
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!roleManager.Roles.Any(r => r.Name.Equals("admin")))
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
        }
    }
}
