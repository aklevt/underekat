using NUnit.Framework;
using UnderEkat.Api.Application;
using UnderEkat.Api.Domain.Entities;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Tests;

public class PublicReadServiceTests
{
    [Test]
    public async Task test_GetPlace_LastPublicCollection()
    {
        var ownerId = Guid.NewGuid();
        var place = new Place(ownerId, "House", "", new Coordinates(56, 60));
        var first = new Collection(ownerId, "Architecture");
        var second = new Collection(ownerId, "Walk");
        first.AddPlace(ownerId, place.Id);
        second.AddPlace(ownerId, place.Id);
        first.ChangeVisibility(ownerId, true);
        second.ChangeVisibility(ownerId, true);
        var source = new TestReadSource { Places = [place], Collections = [first, second] };
        var service = new PublicReadService(source);

        first.ChangeVisibility(ownerId, false);

        Assert.That(await service.GetPlaceAsync(place.Id), Is.Not.Null);
        Assert.That(await service.GetPlacesAsync(null), Has.Length.EqualTo(1));

        second.ChangeVisibility(ownerId, false);

        Assert.That(await service.GetPlaceAsync(place.Id), Is.Null);
        Assert.That(await service.GetPlacesAsync(null), Is.Empty);
        Assert.That(await service.GetPlacesAsync("House"), Is.Empty);
        Assert.That(place.IsDeleted, Is.False);
    }

    [Test]
    public async Task test_GetCollection_Private()
    {
        var collection = new Collection(Guid.NewGuid(), "Personal");
        var source = new TestReadSource { Collections = [collection] };
        var service = new PublicReadService(source);

        Assert.That(await service.GetCollectionsAsync(), Is.Empty);
        Assert.That(await service.GetCollectionAsync(collection.Id), Is.Null);
    }

    [Test]
    public async Task test_GetCollection_Deleted()
    {
        var ownerId = Guid.NewGuid();
        var place = new Place(ownerId, "House", "", new Coordinates(56, 60), address: "Main street, 1");
        var collection = new Collection(ownerId, "Architecture");
        collection.AddPlace(ownerId, place.Id);
        collection.ChangeVisibility(ownerId, true);
        var source = new TestReadSource { Places = [place], Collections = [collection] };
        var service = new PublicReadService(source);

        Assert.That(await service.GetCollectionsAsync(), Has.Length.EqualTo(1));
        Assert.That(await service.GetCollectionAsync(collection.Id), Is.Not.Null);
        Assert.That(await service.GetPlaceAsync(place.Id), Is.Not.Null);
        Assert.That((await service.GetPlaceAsync(place.Id))!.Address, Is.EqualTo("Main street, 1"));

        collection.Delete(ownerId);

        Assert.That(await service.GetCollectionsAsync(), Is.Empty);
        Assert.That(await service.GetCollectionAsync(collection.Id), Is.Null);
        Assert.That(await service.GetPlaceAsync(place.Id), Is.Null);
        Assert.That(await service.GetPlacesAsync(null), Is.Empty);
    }

    [Test]
    public async Task test_GetCollection_DeletedPlace()
    {
        var authorId = Guid.NewGuid();
        var place = new Place(authorId, "House", "Description", new Coordinates(56, 60));
        var collection = new Collection(authorId, "Architecture");
        collection.AddPlace(authorId, place.Id);
        collection.ChangeVisibility(authorId, true);
        var source = new TestReadSource { Places = [place], Collections = [collection] };
        var service = new PublicReadService(source);

        place.Delete(authorId);

        Assert.That(await service.GetPlaceAsync(place.Id), Is.Null);
        Assert.That(await service.GetPlacesAsync(null), Is.Empty);
        Assert.That(await service.GetPlacesAsync("House"), Is.Empty);

        var response = (await service.GetCollectionAsync(collection.Id))!;
        Assert.That(response.Places, Has.Length.EqualTo(1));
        var placeholder = response.Places[0];
        Assert.That(placeholder.Id, Is.EqualTo(place.Id));
        Assert.That(placeholder.IsDeleted, Is.True);
        Assert.That(placeholder.Place, Is.Null);
    }

    /// <summary>
    /// Передает в сервис только данные, подготовленные в тесте
    /// </summary>
    private class TestReadSource : IPlaceReadSource
    {
        public IReadOnlyList<Place> Places { get; set; } = [];
        public IReadOnlyList<Collection> Collections { get; set; } = [];

        public Task<IReadOnlyList<Place>> GetPlacesAsync(IReadOnlyCollection<Guid>? ids = null)
        {
            var places = ids == null ? Places : Places.Where(p => ids.Contains(p.Id)).ToArray();
            return Task.FromResult(places);
        }

        public Task<IReadOnlyList<Collection>> GetCollectionsAsync()
        {
            return Task.FromResult(Collections);
        }
    }
}
