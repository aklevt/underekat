using UnderEkat.Api.Domain.Entities;

namespace UnderEkat.Api.Application;

/// <summary>
/// Источник мест и подборок для чтения API
/// </summary>
public interface IPlaceReadSource
{
    /// <summary>
    /// Возвращает все места или только места с заданными id
    /// </summary>
    Task<IReadOnlyList<Place>> GetPlacesAsync(IReadOnlyCollection<Guid>? ids = null);

    Task<IReadOnlyList<Collection>> GetCollectionsAsync();
}
