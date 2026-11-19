using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Domain.Entities;

public class Place
{
    public Guid Id { get; }
    public Guid AuthorId { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Coordinates Location { get; private set; }
    public bool IsDeleted { get; private set; }

    public Place(Guid authorId, string name, string description, Coordinates location)
        : this(Guid.NewGuid(), authorId, name, description, location)
    {
    }

    /// <summary>
    /// Создает объект места с заданным id
    /// </summary>
    public Place(Guid id, Guid authorId, string name, string description, Coordinates location)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Не указан id места", nameof(id));

        if (authorId == Guid.Empty)
            throw new ArgumentException("Не указан автор места", nameof(authorId));

        ValidateDetails(name, description, location);

        Id = id;
        AuthorId = authorId;
        Name = name.Trim();
        Description = description;
        Location = location;
    }

    /// <summary>
    /// Изменяет название, описание и координаты существующего места только от имени автора
    /// При ошибке проверки сохраняет прежние данные
    /// </summary>
    public void Update(Guid actorId, string name, string description, Coordinates location)
    {
        if (actorId != AuthorId)
            throw new UnauthorizedAccessException("Изменять место может только автор");

        if (IsDeleted)
            throw new InvalidOperationException("Удаленное место нельзя изменять");

        ValidateDetails(name, description, location);

        Name = name.Trim();
        Description = description;
        Location = location;
    }

    /// <summary>
    /// Помечает место удаленным только от имени автора, сохраняя сам объект
    /// Повторное удаление автором не меняет состояние
    /// </summary>
    public void Delete(Guid actorId)
    {
        if (actorId != AuthorId)
            throw new UnauthorizedAccessException("Удалять место может только автор");

        IsDeleted = true;
    }

    private static void ValidateDetails(string name, string description, Coordinates location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название места не может быть пустым", nameof(name));

        if (name.Trim().Length > 65)
            throw new ArgumentException("Название должно содержать не более 65 символов", nameof(name));

        if (description is null)
            throw new ArgumentNullException(nameof(description), "Не указано описание места, допустима пустая строка");

        if (description.Length > 1000)
            throw new ArgumentException("Описание должно содержать не более 1000 символов", nameof(description));

        if (location is null)
            throw new ArgumentNullException(nameof(location), "Не указаны координаты места");
    }
}
