var builder = WebApplication.CreateBuilder(args);

// Escuta em todas as interfaces de rede para acesso via rede local
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// Endpoint de status/saúde da API
app.MapGet("/", () => new { Status = "Online", Timestamp = DateTime.UtcNow })
    .WithName("GetStatus");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
