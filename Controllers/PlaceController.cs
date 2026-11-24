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
    public async Task<ActionResult<PlaceResponse>> GetPlace(Guid id)
    {
        var place = await readService.GetPlaceAsync(id);

        if (place == null)
            return NotFound();

        return Ok(place);
    }

    [HttpGet]
    public async Task<ActionResult<PlaceResponse[]>> GetAll([FromQuery] string? search)
    {
        return Ok(await readService.GetPlacesAsync(search));
    }
}
