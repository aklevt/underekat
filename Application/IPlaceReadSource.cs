using UnderEkat.Api.Domain.Entities;

namespace UnderEkat.Api.Application;

/// <summary>
/// Источник мест и подборок для чтения API
/// </summary>
public interface IPlaceReadSource
{
    IReadOnlyList<Place> Places { get; }
    IReadOnlyList<Collection> Collections { get; }
}
