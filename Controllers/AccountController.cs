using Microsoft.AspNetCore.Mvc;

namespace UnderEkat.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class AccountController : ControllerBase
{
    /*
    // Выдать токен защиты изменяющих запросов.
    [HttpGet("auth/csrf")]
    public IActionResult GetCsrfToken()
    {
        throw new NotImplementedException();
    }

    // Зарегистрировать пользователя и создать сессию.
    [HttpPost("auth/register")]
    public IActionResult Register([FromBody] CredentialsRequest request)
    {
        throw new NotImplementedException();
    }

    // Проверить пароль и создать сессию.
    [HttpPost("auth/login")]
    public IActionResult Login([FromBody] CredentialsRequest request)
    {
        throw new NotImplementedException();
    }

    // Завершить текущую сессию.
    [HttpPost("auth/logout")]
    public IActionResult Logout()
    {
        throw new NotImplementedException();
    }

    // Вернуть данные текущего пользователя.
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        throw new NotImplementedException();
    }

    // Загрузить страницу избранных меток и сохранённых подборок.
    [HttpGet("me/favorites")]
    public IActionResult GetFavorites([FromQuery] string? cursor, [FromQuery] int? limit)
    {
        throw new NotImplementedException();
    }

    // Добавить метку в избранное.
    [HttpPut("me/favorites/places/{placeId}")]
    public IActionResult SaveFavoritePlace(Guid placeId)
    {
        throw new NotImplementedException();
    }

    // Убрать метку из избранного.
    [HttpDelete("me/favorites/places/{placeId}")]
    public IActionResult RemoveFavoritePlace(Guid placeId)
    {
        throw new NotImplementedException();
    }

    // Сохранить чужую подборку.
    [HttpPut("me/favorites/collections/{id}")]
    public IActionResult SaveFavoriteCollection(Guid id)
    {
        throw new NotImplementedException();
    }

    // Убрать подборку из сохранённых.
    [HttpDelete("me/favorites/collections/{id}")]
    public IActionResult RemoveFavoriteCollection(Guid id)
    {
        throw new NotImplementedException();
    }

    public sealed record CredentialsRequest(string Login, string Password);
    */
}
