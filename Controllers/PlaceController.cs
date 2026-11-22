using Microsoft.AspNetCore.Mvc;
using UnderEkat.Api.Contracts;
using UnderEkat.Api.Application;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1/places")]
public class PlaceController : ControllerBase
{
    private readonly PublicReadService readService;

    public PlaceController(PublicReadService readService)
    {
        this.readService = readService;
    }

    [HttpGet("{id}", Name = nameof(GetPlace))]
    public ActionResult<PlaceResponse> GetPlace(Guid id)
    {
        var place = readService.GetPlace(id);

        if (place == null)
            return NotFound();

        return Ok(place);
    }

    [HttpGet]
    public ActionResult<PlaceResponse[]> GetAll([FromQuery] string? search)
    {
        return Ok(readService.GetPlaces(search));
    }
}
