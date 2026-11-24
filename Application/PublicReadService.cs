using UnderEkat.Api.Contracts;
using UnderEkat.Api.Domain.Entities;

namespace UnderEkat.Api.Application;

/// <summary>
/// Возвращает места и подборки, доступные гостю
/// </summary>
public class PublicReadService
{
    private readonly IPlaceReadSource source;

    public PublicReadService(IPlaceReadSource source)
    {
        this.source = source;
    }

    public async Task<PlaceResponse[]> GetPlacesAsync(string? search)
    {
        var query = search?.Trim() ?? "";
        var places = await source.GetPlacesAsync();
        var collections = await source.GetCollectionsAsync();

        return places
            .Where(place => IsPlacePublic(place, collections))
            .Where(place => place.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(ToResponse)
            .ToArray();
    }

    public async Task<PlaceResponse?> GetPlaceAsync(Guid id)
    {
        var places = await source.GetPlacesAsync([id]);
        var place = places.FirstOrDefault();

        if (place == null || place.IsDeleted)
            return null;

        var collections = await source.GetCollectionsAsync();
        return IsPlacePublic(place, collections) ? ToResponse(place) : null;
    }

    public async Task<CollectionSummaryResponse[]> GetCollectionsAsync()
    {
        var collections = await source.GetCollectionsAsync();
        return collections
            .Where(collection => collection.IsPublic && !collection.IsDeleted)
            .Select(collection => new CollectionSummaryResponse
            {
                Id = collection.Id,
                Name = collection.Name
            })
            .ToArray();
    }

    public async Task<CollectionResponse?> GetCollectionAsync(Guid id)
    {
        var collections = await source.GetCollectionsAsync();
        var collection = collections.FirstOrDefault(collection =>
            collection.Id == id && collection.IsPublic && !collection.IsDeleted);

        if (collection == null)
            return null;

        var places = await source.GetPlacesAsync(collection.PlaceIds);

        return new CollectionResponse
        {
            Id = collection.Id,
            Name = collection.Name,
            Places = collection.PlaceIds.Select(placeId => ToCollectionPlace(placeId, places)).ToArray()
        };
    }

    /// <summary>
    /// Проверяет, что место не удалено и входит хотя бы в одну публичную подборку
    /// </summary>
    private static bool IsPlacePublic(Place place, IReadOnlyList<Collection> collections)
    {
        return !place.IsDeleted && collections.Any(collection =>
            collection.IsPublic && !collection.IsDeleted && collection.PlaceIds.Contains(place.Id));
    }

    /// <summary>
    /// Возвращает место подборки или заглушку без подробностей для удаленной ссылки
    /// </summary>
    private static CollectionPlaceResponse ToCollectionPlace(Guid placeId, IReadOnlyList<Place> places)
    {
        var place = places.FirstOrDefault(place => place.Id == placeId);

        if (place == null || place.IsDeleted)
        {
            return new CollectionPlaceResponse
            {
                Id = placeId,
                IsDeleted = true
            };
        }

        return new CollectionPlaceResponse
        {
            Id = placeId,
            Place = ToResponse(place)
        };
    }

    private static PlaceResponse ToResponse(Place place)
    {
        return new PlaceResponse
        {
            Id = place.Id,
            Name = place.Name,
            Description = place.Description,
            Address = place.Address,
            Latitude = place.Location.Latitude,
            Longitude = place.Location.Longitude,
            Access = place.Access,
            Guard = place.Guard,
            Tags = place.Tags.ToArray()
        };
    }
}
