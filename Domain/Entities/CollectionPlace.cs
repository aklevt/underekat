namespace UnderEkat.Api.Domain.Entities;

/// <summary>
/// Связь места с подборкой, хранится в БД в таблице collection_places
/// </summary>
public class CollectionPlace
{
    public Guid CollectionId { get; }
    public Guid PlaceId { get; }

    internal CollectionPlace(Guid collectionId, Guid placeId)
    {
        CollectionId = collectionId;
        PlaceId = placeId;
    }
}
