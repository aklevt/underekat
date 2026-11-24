using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UnderEkat.Api.Application;
using UnderEkat.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, allowIntegerValues: false));
});
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalFrontend", policy =>
        policy.WithOrigins("http://localhost:18080")
            .WithMethods("GET"));
});
builder.Services.AddDbContext<UnderEkatDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("Не настроено подключение к PostgreSQL");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<IPlaceReadSource, EfPlaceReadSource>();
builder.Services.AddScoped<PublicReadService>();
builder.Services.AddScoped<PlaceImporter>();

var app = builder.Build();

if (args.Length > 0 && args[0] == "import")
{
    if (args.Length != 2)
    {
        Console.Error.WriteLine("Формат: import places.json");
        Environment.ExitCode = 1;
        return;
    }

    await using var scope = app.Services.CreateAsyncScope();
    try
    {
        var importer = scope.ServiceProvider.GetRequiredService<PlaceImporter>();
        var added = await importer.ImportAsync(args[1]);
        Console.WriteLine($"Добавлено мест: {added}");
    }
    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
        or JsonException or ArgumentException or InvalidOperationException or DbUpdateException or NpgsqlException)
    {
        var message = exception switch
        {
            IOException => "Не удалось прочитать файл для импорта",
            UnauthorizedAccessException => "Недостаточно прав для импорта",
            JsonException => "Неверный формат JSON для импорта",
            DbUpdateException => "Не удалось сохранить данные импорта в БД",
            NpgsqlException => "Не удалось выполнить запрос к БД",
            _ => exception.Message
        };
        Console.Error.WriteLine(message);
        Environment.ExitCode = 1;
    }

    return;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("LocalFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
