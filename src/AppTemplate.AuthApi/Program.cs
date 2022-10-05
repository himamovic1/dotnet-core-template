using AppTemplate.AuthApi.Extensions.Startup;
using Microsoft.AspNetCore.Builder;
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
builder.Services.AddIdentityServerConfig(builder.Configuration);
builder.Services.AddServices();

var app = builder.Build();

app.UseSwaggerConfig();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseIdentityServer();

app.MapControllers();
app.MapHealthChecks("/hc");

app.Run();
