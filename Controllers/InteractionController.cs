using Microsoft.AspNetCore.Mvc;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class InteractionController : ControllerBase
{
    /*
    // Удалить собственный комментарий.
    [HttpDelete("comments/{id}")]
    public IActionResult DeleteComment(Guid id)
    {
        throw new NotImplementedException();
    }

    // Открыть подборку по действующей ссылке доступа.
    [HttpGet("shared/{token}")]
    public IActionResult OpenSharedCollection(string token)
    {
        throw new NotImplementedException();
    }

    // Записать клиентское событие по контракту.
    [HttpPost("events")]
    public IActionResult RecordEvent([FromBody] ClientEventRequest request)
    {
        throw new NotImplementedException();
    }

    public sealed record ClientEventRequest(Guid EventId, string Type, string? TargetType,
        Guid? TargetId, string? UtmSource);
    */
}
