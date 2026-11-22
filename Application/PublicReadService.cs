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

    public PlaceResponse[] GetPlaces(string? search)
    {
        var query = search?.Trim() ?? "";

        return source.Places
            .Where(IsPlacePublic)
            .Where(place => place.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(ToResponse)
            .ToArray();
    }

    public PlaceResponse? GetPlace(Guid id)
    {
        var place = source.Places.FirstOrDefault(place => place.Id == id);
        return place != null && IsPlacePublic(place) ? ToResponse(place) : null;
    }

    public CollectionSummaryResponse[] GetCollections()
    {
        return source.Collections
            .Where(collection => collection.IsPublic && !collection.IsDeleted)
            .Select(collection => new CollectionSummaryResponse
            {
                Id = collection.Id,
                Name = collection.Name
            })
            .ToArray();
    }

    public CollectionResponse? GetCollection(Guid id)
    {
        var collection = source.Collections.FirstOrDefault(collection =>
            collection.Id == id && collection.IsPublic && !collection.IsDeleted);

        if (collection == null)
            return null;

        return new CollectionResponse
        {
            Id = collection.Id,
            Name = collection.Name,
            Places = collection.PlaceIds.Select(ToCollectionPlace).ToArray()
        };
    }

    /// <summary>
    /// Проверяет, что место не удалено и входит хотя бы в одну публичную подборку
    /// </summary>
    private bool IsPlacePublic(Place place)
    {
        return !place.IsDeleted && source.Collections.Any(collection =>
            collection.IsPublic && !collection.IsDeleted && collection.PlaceIds.Contains(place.Id));
    }

    /// <summary>
    /// Возвращает место подборки или заглушку без подробностей для удаленной ссылки
    /// </summary>
    private CollectionPlaceResponse ToCollectionPlace(Guid placeId)
    {
        var place = source.Places.FirstOrDefault(place => place.Id == placeId);

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
