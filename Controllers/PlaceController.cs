using Microsoft.AspNetCore.Mvc;
using UnderEkat.Api.Contracts;
using UnderEkat.Api.Domain.Entities;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1/places")]
public class PlaceController : ControllerBase
{
    private static readonly Guid DemoAuthorId = Guid.Parse("bb000000-0000-4000-8000-000000000001");

    private static readonly Place[] Places =
    [
        new(
            Guid.Parse("aa000000-0000-4000-8000-000000000001"),
            DemoAuthorId,
            "First place",
            "Desc of test place",
            new Coordinates(56.8389, 60.6057)),
        new(
            Guid.Parse("aa000000-0000-4000-8000-000000000002"),
            DemoAuthorId,
            "Second place",
            "Desc of test place",
            new Coordinates(56.8400, 60.6100))
    ];

    [HttpGet("{id}", Name = nameof(GetPlace))]
    public ActionResult<PlaceResponse> GetPlace(Guid id)
    {
        var place = Places.FirstOrDefault(place => place.Id == id && !place.IsDeleted);

        if (place == null)
            return NotFound();

        return Ok(ToResponse(place));
    }

    [HttpGet]
    public ActionResult<PlaceResponse[]> GetAll([FromQuery] string? search)
    {
        var query = search?.Trim() ?? "";

        var places = Places
            .Where(place => !place.IsDeleted)
            .Where(place => place.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(ToResponse)
            .ToArray();

        return Ok(places);
    }

    /// <summary>
    /// Преобразует объект Place в формат ответа для API
    /// </summary>
    private static PlaceResponse ToResponse(Place place)
    {
        return new PlaceResponse
        {
            Id = place.Id,
            Name = place.Name,
            Description = place.Description,
            Latitude = place.Location.Latitude,
            Longitude = place.Location.Longitude
        };
    }
}
