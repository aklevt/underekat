namespace UnderEkat.Api.Domain.Entities;

/// <summary>
/// Связь места с подборкой, хранится в БД в таблице collection_places
/// </summary>
public class CollectionPlaceLink
{
    public Guid CollectionId { get; }
    public Guid PlaceId { get; }

    internal CollectionPlaceLink(Guid collectionId, Guid placeId)
    {
        CollectionId = collectionId;
        PlaceId = placeId;
    }
}
