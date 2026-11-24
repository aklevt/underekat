using Microsoft.AspNetCore.Mvc;
using UnderEkat.Api.Application;
using UnderEkat.Api.Contracts;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1/collections")]
public class CollectionController : ControllerBase
{
    private readonly PublicReadService readService;

    public CollectionController(PublicReadService readService)
    {
        this.readService = readService;
    }

    [HttpGet]
    public async Task<ActionResult<CollectionSummaryResponse[]>> GetAll()
    {
        return Ok(await readService.GetCollectionsAsync());
    }

    [HttpGet("{id}", Name = nameof(GetCollection))]
    public async Task<ActionResult<CollectionResponse>> GetCollection(Guid id)
    {
        var collection = await readService.GetCollectionAsync(id);

        if (collection == null)
            return NotFound();

        return Ok(collection);
    }
}
