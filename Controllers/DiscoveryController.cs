using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UnderEkat.Api.Application;
using UnderEkat.Api.Contracts;
using UnderEkat.Api.Domain.Enums;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class DiscoveryController(PublicReadService readService) : ControllerBase
{
    [HttpGet("map/places")]
    public IActionResult GetMap(
        [FromQuery, Required] string bbox,
        [FromQuery] Guid[] collectionId,
        [FromQuery] string[] tag,
        [FromQuery] string[] access,
        [FromQuery] string[] guard,
        [FromQuery] bool freshOnly = false)
    {
        var parts = bbox.Split(',');
        if (parts.Length != 4 || parts.Any(part => !double.TryParse(
            part, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value)))
            return Problem(statusCode: 400, detail: "Expected bbox=minLng,minLat,maxLng,maxLat.");

        var bounds = parts.Select(part => double.Parse(part, CultureInfo.InvariantCulture)).ToArray();
        if (bounds[0] < -180 || bounds[2] > 180 || bounds[1] < -90 || bounds[3] > 90 ||
            bounds[0] >= bounds[2] || bounds[1] >= bounds[3])
            return Problem(statusCode: 400, detail: "Invalid map bounds.");

        if (!ValidFilters<PlaceAccess>(access) || !ValidFilters<GuardStatus>(guard))
            return Problem(statusCode: 400, detail: "Invalid access or guard filter.");

        var collections = readService.GetCollections()
            .Select(collection => readService.GetCollection(collection.Id)!)
            .ToArray();
        var collectionIds = collections.Select(collection => collection.Id).ToHashSet();
        if (collectionId.Any(id => !collectionIds.Contains(id)))
            return NotFound();

        var membership = collections.SelectMany(collection => collection.Places
            .Where(item => !item.IsDeleted)
            .Select(item => (PlaceId: item.Id, CollectionId: collection.Id)))
            .ToLookup(item => item.PlaceId, item => item.CollectionId);

        // The current source has no freshness checks. All places are unconfirmed.
        var places = freshOnly ? Array.Empty<PlaceResponse>() : readService.GetPlaces(null);
        var features = places.Where(place =>
                place.Longitude >= bounds[0] && place.Longitude <= bounds[2] &&
                place.Latitude >= bounds[1] && place.Latitude <= bounds[3] &&
                (collectionId.Length == 0 || membership[place.Id].Intersect(collectionId).Any()) &&
                tag.All(value => place.Tags.Contains(value, StringComparer.OrdinalIgnoreCase)) &&
                MatchesFilter(place.Access, access) && MatchesFilter(place.Guard, guard))
            .Take(5001)
            .Select(place => new
            {
                type = "Feature",
                place.Id,
                geometry = new { type = "Point", coordinates = new[] { place.Longitude, place.Latitude } },
                properties = new
                {
                    place.Id, place.Name, place.Address, place.Tags, place.Access, place.Guard,
                    freshnessState = "unconfirmed",
                    collectionIds = membership[place.Id].ToArray()
                }
            }).ToArray();

        if (features.Length > 5000)
            return Problem(statusCode: 422, detail: "Reduce the map bounds.",
                extensions: new Dictionary<string, object?> { ["code"] = "MAP_RESULT_TOO_LARGE" });

        return Ok(new { type = "FeatureCollection", features });
    }

    [HttpGet("search")]
    public IActionResult Search(
        [FromQuery, Required, StringLength(100, MinimumLength = 2)] string q,
        [FromQuery] string[] types,
        [FromQuery, Range(1, 10)] int limit = 5)
    {
        var query = q.Trim();
        if (query.Length < 2 || types.Any(type => type is not ("places" or "collections" or "tags")))
            return Problem(statusCode: 400, detail: "Invalid search query or result type.");

        var tagsOnly = query.StartsWith('#');
        var places = readService.GetPlaces(null);
        return Ok(new
        {
            places = places.Where(place => !tagsOnly && Includes("places") &&
                    (Contains(place.Name) || Contains(place.Address) || Contains(place.Description) ||
                     place.Tags.Any(Contains)))
                .Take(limit)
                .Select(place => new
                {
                    place.Id, place.Name, place.Address,
                    location = new { place.Latitude, place.Longitude },
                    place.Tags, place.Access, place.Guard,
                    freshnessState = "unconfirmed",
                    coverUrl = (string?)null
                }).ToArray(),
            collections = readService.GetCollections()
                .Where(collection => !tagsOnly && Includes("collections") && Contains(collection.Name))
                .Take(limit).ToArray(),
            tags = Includes("tags") ? SuggestTags(places, query.TrimStart('#'), limit) : []
        });

        bool Includes(string type) => types.Length == 0 || types.Contains(type);
        bool Contains(string value) => value.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    [HttpGet("tags")]
    public IActionResult GetTags(
        [FromQuery, StringLength(40)] string? prefix,
        [FromQuery, Range(1, 10)] int limit = 10)
    {
        return Ok(SuggestTags(readService.GetPlaces(null), prefix?.Trim() ?? "", limit));
    }

    private static TagSuggestion[] SuggestTags(IEnumerable<PlaceResponse> places, string prefix, int limit)
    {
        return places.SelectMany(place => place.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
            .Where(tag => tag.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .GroupBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .Select(group => new TagSuggestion(group.Key, group.Count()))
            .OrderByDescending(tag => tag.Count)
            .ThenBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)
            .Take(limit)
            .ToArray();
    }

    private static bool ValidFilters<T>(string[] filters) where T : struct, Enum =>
        filters.All(filter => Enum.GetValues<T>().Any(value => WireName(value) == filter));

    private static bool MatchesFilter<T>(T value, string[] filters) where T : struct, Enum =>
        filters.Length == 0 || filters.Contains(WireName(value));

    private static string WireName<T>(T value) where T : struct, Enum =>
        JsonNamingPolicy.SnakeCaseLower.ConvertName(value.ToString());

    private sealed record TagSuggestion(string Name, int Count);
}
