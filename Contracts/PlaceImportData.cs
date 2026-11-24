using UnderEkat.Api.Domain.Enums;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Contracts;

/// <summary>
/// Данные JSON-файла для импорта мест и подборок
/// </summary>
public class PlaceImportData
{
    public required CollectionData[] Collections { get; init; }
    public required PlaceData[] Places { get; init; }

    public class CollectionData
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required bool IsPublic { get; init; }
    }

    public class PlaceData
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string Address { get; init; }
        public required Coordinates Location { get; init; }
        public required PlaceAccess Access { get; init; }
        public required GuardStatus Guard { get; init; }
        public required Guid[] CollectionIds { get; init; }
    }
}
