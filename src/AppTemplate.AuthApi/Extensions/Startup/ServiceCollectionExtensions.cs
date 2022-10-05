using Microsoft.Extensions.DependencyInjection;

namespace AppTemplate.AuthApi.Extensions.Startup;

/// <summary>
/// Extension methods to simplify adding of services to <c>IServiceCollection</c> in the <c>Program.cs</c>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Extension method that adds all other services and components.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // services.AddScoped<IService, ServiceImplementation>();
        // services.AddSingleton<ISingletonService, SingletonServiceImplementation>();

        return services;
    }
}
