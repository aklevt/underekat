using System.Text.Json;
using System.Text.Json.Serialization;
using UnderEkat.Api.Application;
using UnderEkat.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, allowIntegerValues: false));
});
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPlaceReadSource, DemoPlaceReadSource>();
builder.Services.AddScoped<PublicReadService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
