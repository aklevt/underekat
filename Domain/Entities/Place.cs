using UnderEkat.Api.Domain.Enums;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Domain.Entities;

public class Place
{
    public Guid Id { get; }
    public Guid AuthorId { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    /// <summary>
    /// Адрес или заданный пользователем текстовый ориентир, допустима пустая строка
    /// </summary>
    public string Address { get; private set; }

    public Coordinates Location { get; private set; }
    public PlaceAccess Access { get; }
    public GuardStatus Guard { get; }
    public IReadOnlyList<string> Tags { get; } = [];
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Конструктор для EF Core, используется при чтении места из БД
    /// Его EF Core вызывает и заполняет свойства сохраненными значениями
    /// </summary>
    private Place()
    {
        Name = null!;
        Description = null!;
        Address = null!;
        Location = null!;
    }

    public Place(
        Guid authorId,
        string name,
        string description,
        Coordinates location,
        PlaceAccess access = PlaceAccess.Unknown,
        GuardStatus guard = GuardStatus.Unknown,
        string address = ""
    )
        : this(Guid.NewGuid(), authorId, name, description, location, access, guard, address) { }

    /// <summary>
    /// Создает объект места с заданным id
    /// </summary>
    public Place(
        Guid id,
        Guid authorId,
        string name,
        string description,
        Coordinates location,
        PlaceAccess access = PlaceAccess.Unknown,
        GuardStatus guard = GuardStatus.Unknown,
        string address = ""
    )
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Не указан id места", nameof(id));

        if (authorId == Guid.Empty)
            throw new ArgumentException("Не указан автор места", nameof(authorId));

        ValidateDetails(name, description, location, address);

        if (!Enum.IsDefined(access))
            throw new ArgumentException("Неизвестное значение доступности места", nameof(access));

        if (!Enum.IsDefined(guard))
            throw new ArgumentException("Неизвестное значение охраняемости места", nameof(guard));

        Id = id;
        AuthorId = authorId;
        Name = name.Trim();
        Description = description;
        Address = address.Trim();
        Location = location;
        Access = access;
        Guard = guard;
    }

    /// <summary>
    /// Изменяет название, описание, адрес и координаты существующего места только от имени автора
    /// При ошибке проверки сохраняет прежние данные
    /// </summary>
    public void Update(
        Guid actorId,
        string name,
        string description,
        Coordinates location,
        string address
    )
    {
        if (actorId != AuthorId)
            throw new UnauthorizedAccessException("Изменять место может только автор");

        if (IsDeleted)
            throw new InvalidOperationException("Удаленное место нельзя изменять");

        ValidateDetails(name, description, location, address);

        Name = name.Trim();
        Description = description;
        Address = address.Trim();
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

    private static void ValidateDetails(
        string name,
        string description,
        Coordinates location,
        string address
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название места не может быть пустым", nameof(name));

        if (name.Trim().Length > 65)
            throw new ArgumentException(
                "Название должно содержать не более 65 символов",
                nameof(name)
            );

        if (description is null)
            throw new ArgumentNullException(
                nameof(description),
                "Не указано описание места, допустима пустая строка"
            );

        if (description.Length > 1000)
            throw new ArgumentException(
                "Описание должно содержать не более 1000 символов",
                nameof(description)
            );

        if (location is null)
            throw new ArgumentNullException(nameof(location), "Не указаны координаты места");

        if (address is null)
            throw new ArgumentNullException(nameof(address), "Не задан адрес места");
    }
}
