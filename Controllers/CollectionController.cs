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
    public ActionResult<CollectionSummaryResponse[]> GetAll()
    {
        return Ok(readService.GetCollections());
    }

    [HttpGet("{id}", Name = nameof(GetCollection))]
    public ActionResult<CollectionResponse> GetCollection(Guid id)
    {
        var collection = readService.GetCollection(id);

        if (collection == null)
            return NotFound();

        return Ok(collection);
    }

    /*
    // Создать подборку.
    [HttpPost]
    public IActionResult CreateCollection([FromBody] CreateCollectionRequest request)
    {
        throw new NotImplementedException();
    }

    // Сохранить настройки подборки и её видимость.
    [HttpPatch("{id}")]
    public IActionResult UpdateCollection(Guid id, [FromBody] PatchCollectionRequest request)
    {
        throw new NotImplementedException();
    }

    // Удалить подборку от имени владельца.
    [HttpDelete("{id}")]
    public IActionResult DeleteCollection(Guid id)
    {
        throw new NotImplementedException();
    }

    // Добавить метку в подборку.
    [HttpPut("{id}/places/{placeId}")]
    public IActionResult AddPlace(Guid id, Guid placeId)
    {
        throw new NotImplementedException();
    }

    // Убрать метку из подборки.
    [HttpDelete("{id}/places/{placeId}")]
    public IActionResult RemovePlace(Guid id, Guid placeId)
    {
        throw new NotImplementedException();
    }

    // Добавить несколько выбранных меток.
    [HttpPost("{id}/places")]
    public IActionResult AddPlaces(Guid id, [FromBody] BatchPlacesRequest request)
    {
        throw new NotImplementedException();
    }

    // Создать ссылку доступа к подборке.
    [HttpPost("{id}/share-links")]
    public IActionResult CreateShareLink(Guid id)
    {
        throw new NotImplementedException();
    }

    // Отозвать ссылки доступа к подборке.
    [HttpDelete("{id}/share-links")]
    public IActionResult RevokeShareLinks(Guid id)
    {
        throw new NotImplementedException();
    }

    public sealed record CreateCollectionRequest(string Name, string? Description,
        string? Color, string? Icon, string? Visibility);
    public sealed record PatchCollectionRequest(string? Name, string? Description,
        string? Color, string? Icon, string? Visibility);
    public sealed record BatchPlacesRequest(Guid[] PlaceIds);
    */
}
