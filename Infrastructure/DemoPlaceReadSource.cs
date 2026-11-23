using UnderEkat.Api.Application;
using UnderEkat.Api.Domain.Entities;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Infrastructure;

/// <summary>
/// Хранит тестовые места и подборки в памяти для демонстрации работы api (до подключения БД)
/// </summary>
public class DemoPlaceReadSource : IPlaceReadSource
{
    public IReadOnlyList<Place> Places { get; }
    public IReadOnlyList<Collection> Collections { get; }

    public DemoPlaceReadSource()
    {
        var authorId = Guid.Parse("bb000000-0000-4000-8000-000000000001");
        var first = new Place(
            Guid.Parse("aa000000-0000-4000-8000-000000000001"),
            authorId, "First place", "Desc of test place", new Coordinates(56.8389, 60.6057),
            address: "Адрес первого места в центре города");
        var second = new Place(
            Guid.Parse("aa000000-0000-4000-8000-000000000002"),
            authorId, "Second place", "Desc of test place", new Coordinates(56.8400, 60.6100),
            address: "Адрес второго места в центре города");
        var deleted = new Place(
            Guid.Parse("aa000000-0000-4000-8000-000000000004"),
            authorId, "Удаленное место", "Описание удаленного места", new Coordinates(56.83, 60.61));
        var privatePlace = new Place(
            Guid.Parse("aa000000-0000-4000-8000-000000000005"),
            authorId, "Приватное место", "Личная заметка", new Coordinates(56.84, 60.62));

        var architecture = new Collection(
            Guid.Parse("cc000000-0000-4000-8000-000000000001"), authorId, "Архитектура");
        architecture.AddPlace(authorId, first.Id);
        architecture.AddPlace(authorId, deleted.Id);
        architecture.ChangeVisibility(authorId, true);

        var walk = new Collection(
            Guid.Parse("cc000000-0000-4000-8000-000000000002"), authorId, "Прогулка");
        walk.AddPlace(authorId, first.Id);
        walk.AddPlace(authorId, second.Id);
        walk.ChangeVisibility(authorId, true);

        var personal = new Collection(
            Guid.Parse("cc000000-0000-4000-8000-000000000003"), authorId, "Личные заметки");
        personal.AddPlace(authorId, privatePlace.Id);

        deleted.Delete(authorId);

        Places = Array.AsReadOnly(new[] { first, second, deleted, privatePlace });
        Collections = Array.AsReadOnly(new[] { architecture, walk, personal });
    }
}
