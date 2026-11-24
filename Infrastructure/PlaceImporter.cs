using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using UnderEkat.Api.Contracts;
using UnderEkat.Api.Domain.Entities;

namespace UnderEkat.Api.Infrastructure;

/// <summary>
/// Загружает места и подборки из JSON от имени служебного пользователя
/// </summary>
public class PlaceImporter
{
    private static readonly Guid authorId = Guid.Parse("bb000000-0000-4000-8000-000000000001");
    private readonly UnderEkatDbContext db;

    public PlaceImporter(UnderEkatDbContext db)
    {
        this.db = db;
    }

    /// <summary>
    /// Сохраняет новые места, подборки и связи, пропуская существующие id
    /// Возвращает количество добавленных мест
    /// </summary>
    public async Task<int> ImportAsync(string path)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, allowIntegerValues: false));

        await using var file = File.OpenRead(path);
        var data = await JsonSerializer.DeserializeAsync<PlaceImportData>(file, options);

        if (data?.Collections is null || data.Places is null)
            throw new InvalidOperationException("Не указаны места или подборки для импорта");

        if (data.Collections.Any(c => c is null) || data.Places.Any(p => p is null || p.CollectionIds is null))
            throw new InvalidOperationException("Не заполнены данные импорта");

        if (data.Collections.Select(c => c.Id).Distinct().Count() != data.Collections.Length
            || data.Places.Select(p => p.Id).Distinct().Count() != data.Places.Length)
            throw new InvalidOperationException("В файле есть повторы id у мест или подборок");

        var collections = await db.Collections.ToDictionaryAsync(c => c.Id);
        var existingPlaceIds = (await db.Places.Select(p => p.Id).ToListAsync()).ToHashSet();

        foreach (var item in data.Collections)
        {
            if (collections.ContainsKey(item.Id))
                continue;

            var collection = new Collection(item.Id, authorId, item.Name);
            collection.ChangeVisibility(authorId, item.IsPublic);
            collections.Add(collection.Id, collection);
            db.Collections.Add(collection);
        }

        var added = 0;
        foreach (var item in data.Places)
        {
            if (existingPlaceIds.Contains(item.Id))
                continue;

            var place = new Place(item.Id, authorId, item.Name, item.Description, item.Location,
                item.Access, item.Guard, item.Address);

            foreach (var collectionId in item.CollectionIds)
            {
                if (!collections.TryGetValue(collectionId, out var collection))
                    throw new InvalidOperationException($"Не найдена подборка {collectionId}");

                collection.AddPlace(authorId, place.Id);
            }

            db.Places.Add(place);
            added++;
        }

        await db.SaveChangesAsync();
        return added;
    }
}
