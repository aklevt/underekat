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

    /*
    // Создать метку и добавить её в выбранные подборки.
    [HttpPost]
    public IActionResult CreatePlace([FromBody] CreatePlaceRequest request)
    {
        throw new NotImplementedException();
    }

    // Сохранить изменения карточки метки.
    [HttpPatch("{id}")]
    public IActionResult UpdatePlace(Guid id, [FromBody] PatchPlaceRequest request)
    {
        throw new NotImplementedException();
    }

    // Мягко удалить метку от имени автора.
    [HttpDelete("{id}")]
    public IActionResult DeletePlace(Guid id)
    {
        throw new NotImplementedException();
    }

    // Перенести маркер и сохранить адрес.
    [HttpPut("{id}/location")]
    public IActionResult MovePlace(Guid id, [FromBody] MovePlaceRequest request)
    {
        throw new NotImplementedException();
    }

    // Проверить файл, сохранить оригинал и миниатюру.
    [HttpPost("{id}/photos")]
    public IActionResult UploadPhoto(Guid id, [FromForm] IFormFile file)
    {
        throw new NotImplementedException();
    }

    // Удалить фотографию от имени автора метки.
    [HttpDelete("{id}/photos/{photoId}")]
    public IActionResult DeletePhoto(Guid id, Guid photoId)
    {
        throw new NotImplementedException();
    }

    // Отдать оригинал после проверки видимости метки.
    [HttpGet("{id}/photos/{photoId}/content")]
    public IActionResult GetPhoto(Guid id, Guid photoId)
    {
        throw new NotImplementedException();
    }

    // Отдать миниатюру после проверки видимости метки.
    [HttpGet("{id}/photos/{photoId}/thumbnail")]
    public IActionResult GetThumbnail(Guid id, Guid photoId)
    {
        throw new NotImplementedException();
    }

    // Записать оценку актуальности.
    [HttpPost("{id}/checks")]
    public IActionResult AddCheck(Guid id, [FromBody] CheckRequest request)
    {
        throw new NotImplementedException();
    }

    // Отменить собственную оценку.
    [HttpDelete("{id}/checks/mine")]
    public IActionResult UndoCheck(Guid id)
    {
        throw new NotImplementedException();
    }

    // Загрузить страницу комментариев к видимой метке.
    [HttpGet("{id}/comments")]
    public IActionResult GetComments(Guid id, [FromQuery] string? cursor, [FromQuery] int? limit)
    {
        throw new NotImplementedException();
    }

    // Добавить комментарий от текущего пользователя или гостя.
    [HttpPost("{id}/comments")]
    public IActionResult AddComment(Guid id, [FromBody] CreateCommentRequest request)
    {
        throw new NotImplementedException();
    }

    public sealed record LocationRequest(double Lat, double Lng);
    public sealed record CreatePlaceRequest(string Name, string Address, string? Description,
        string? History, Domain.Enums.PlaceAccess? Access, Domain.Enums.GuardStatus? Guard, string[]? Tags,
        LocationRequest Location, Guid[]? CollectionIds);
    public sealed record PatchPlaceRequest(string? Name, string? Address, string? Description,
        string? History, Domain.Enums.PlaceAccess? Access, Domain.Enums.GuardStatus? Guard, string[]? Tags);
    public sealed record MovePlaceRequest(LocationRequest Location, string? Address);
    public sealed record CheckRequest(bool IsActual);
    public sealed record CreateCommentRequest(string Body);
    */
}
