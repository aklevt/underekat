using Microsoft.AspNetCore.Mvc;
using UnderEkat.Api.Contracts;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1/places")]
public class PlaceController : ControllerBase
{
    private static readonly PlaceResponse[] Places =
    [
        new()
        {
            Id = Guid.Parse("aa000000-0000-4000-8000-000000000001"),
            Name = "First place",
            Description = "Desc of test place",
            Latitude = 56.8389,
            Longitude = 60.6057
        },
        new()
        {
            Id = Guid.Parse("aa000000-0000-4000-8000-000000000002"),
            Name = "Second place",
            Description = "Desc of test place",
            Latitude = 56.8400,
            Longitude = 60.6100
        }
    ];

    [HttpGet("{id}", Name = nameof(GetPlace))]
    public ActionResult<PlaceResponse> GetPlace(Guid id)
    {
        var place = Places.FirstOrDefault(place => place.Id == id);

        if (place == null)
        {
            return NotFound();
        }

        return Ok(place);
    }

    [HttpGet]
    public ActionResult<PlaceResponse[]> GetAll([FromQuery] string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return Ok(Places);
        }

        var places = Places
            .Where(place => place.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return Ok(places);
    }
}
