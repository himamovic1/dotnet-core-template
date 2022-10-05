using AppTemplate.AuthApi.Database.Contexts;
using AppTemplate.AuthApi.Extensions.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

builder.Services.AddSwaggerConfig();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AuthApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthApiDbContext")));

builder.Services.AddServices();
builder.Services.AddIdentityServerConfig(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseSwaggerConfig();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseIdentityServer();
await app.UseIdentityServerDataAsync(app.Configuration);

app.MapControllers();
app.MapHealthChecks("/hc");
app.UseCors("CORS");

app.Run();

// Make Program class visible
namespace AppTemplate
{
    public partial class Program { }
}