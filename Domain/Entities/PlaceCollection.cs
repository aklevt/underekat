namespace UnderEkat.Api.Domain.Entities;

/// <summary>
/// Обычная подборка, составом которой управляет владелец
/// Хранит ссылки на места по id, не управляя самими местами
/// </summary>
public class PlaceCollection
{
    private readonly List<Guid> placeIds = [];

    public Guid Id { get; }
    public Guid OwnerId { get; }
    public string Name { get; }
    public bool IsPublic { get; private set; }
    public bool IsDeleted { get; private set; }
    public IReadOnlyList<Guid> PlaceIds { get; }

    public PlaceCollection(Guid ownerId, string name)
        : this(Guid.NewGuid(), ownerId, name) { }

    public PlaceCollection(Guid id, Guid ownerId, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Не указан id подборки", nameof(id));

        if (ownerId == Guid.Empty)
            throw new ArgumentException("Не указан владелец подборки", nameof(ownerId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название подборки не может быть пустым", nameof(name));

        if (name.Trim().Length > 55)
            throw new ArgumentException(
                "Название подборки должно содержать не более 55 символов",
                nameof(name)
            );

        Id = id;
        OwnerId = ownerId;
        Name = name.Trim();
        PlaceIds = placeIds.AsReadOnly();
    }

    /// <summary>
    /// Добавляет ссылку от имени владельца, если ее еще нет в подборке
    /// </summary>
    public void AddPlace(Guid actorId, Guid placeId)
    {
        EnsureCanChange(actorId);

        if (placeId == Guid.Empty)
            throw new ArgumentException("Не указан id места", nameof(placeId));

        if (!placeIds.Contains(placeId))
            placeIds.Add(placeId);
    }

    /// <summary>
    /// Убирает ссылку на место от имени владельца, сохраняя само место
    /// Повторное удаление ссылки не меняет состав подборки
    /// </summary>
    public void RemovePlace(Guid actorId, Guid placeId)
    {
        EnsureCanChange(actorId);

        if (placeId == Guid.Empty)
            throw new ArgumentException("Не указан id места", nameof(placeId));

        placeIds.Remove(placeId);
    }

    /// <summary>
    /// Помечает подборку удаленной от имени владельца, сохраняя места
    /// Повторное удаление владельцем не меняет состояние подборки
    /// </summary>
    public void Delete(Guid actorId)
    {
        if (actorId != OwnerId)
            throw new UnauthorizedAccessException("Удалять подборку может только владелец");

        IsDeleted = true;
    }

    /// <summary>
    /// Изменяет публичность подборки только от имени владельца
    /// </summary>
    public void ChangeVisibility(Guid actorId, bool isPublic)
    {
        EnsureCanChange(actorId);
        IsPublic = isPublic;
    }

    /// <summary>
    /// Проверяет право владельца изменять существующую подборку
    /// </summary>
    private void EnsureCanChange(Guid actorId)
    {
        if (actorId != OwnerId)
            throw new UnauthorizedAccessException("Изменять подборку может только владелец");

        if (IsDeleted)
            throw new InvalidOperationException("Удаленную подборку нельзя изменять");
    }
}
