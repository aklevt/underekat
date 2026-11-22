using Microsoft.AspNetCore.Mvc;
using UnderEkat.Api.Application;
using UnderEkat.Api.Contracts;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1/collections")]
public class CollectionController : ControllerBase
{
    private readonly PublicReadService queries;

    public CollectionController(PublicReadService queries)
    {
        this.queries = queries;
    }

    [HttpGet]
    public ActionResult<CollectionSummaryResponse[]> GetAll()
    {
        return Ok(queries.GetCollections());
    }

    [HttpGet("{id}", Name = nameof(GetCollection))]
    public ActionResult<CollectionResponse> GetCollection(Guid id)
    {
        var collection = queries.GetCollection(id);

        if (collection == null)
            return NotFound();

        return Ok(collection);
    }
}
