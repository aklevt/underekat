using NUnit.Framework;
using UnderEkat.Api.Application;
using UnderEkat.Api.Domain.Entities;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Tests;

public class PublicReadServiceTests
{
    [Test]
    public void test_GetPlace_LastPublicCollection()
    {
        var ownerId = Guid.NewGuid();
        var place = new Place(ownerId, "House", "", new Coordinates(56, 60));
        var first = new PlaceCollection(ownerId, "Architecture");
        var second = new PlaceCollection(ownerId, "Walk");
        first.AddPlace(ownerId, place.Id);
        second.AddPlace(ownerId, place.Id);
        first.ChangeVisibility(ownerId, true);
        second.ChangeVisibility(ownerId, true);
        var source = new TestReadSource { Places = [place], Collections = [first, second] };
        var service = new PublicReadService(source);

        first.ChangeVisibility(ownerId, false);

        Assert.That(service.GetPlace(place.Id), Is.Not.Null);
        Assert.That(service.GetPlaces(null), Has.Length.EqualTo(1));

        second.ChangeVisibility(ownerId, false);

        Assert.That(service.GetPlace(place.Id), Is.Null);
        Assert.That(service.GetPlaces(null), Is.Empty);
        Assert.That(service.GetPlaces("House"), Is.Empty);
        Assert.That(place.IsDeleted, Is.False);
    }

    [Test]
    public void test_GetCollection_Private()
    {
        var collection = new PlaceCollection(Guid.NewGuid(), "Personal");
        var source = new TestReadSource { Collections = [collection] };
        var service = new PublicReadService(source);

        Assert.That(service.GetCollections(), Is.Empty);
        Assert.That(service.GetCollection(collection.Id), Is.Null);
    }

    [Test]
    public void test_GetCollection_Deleted()
    {
        var ownerId = Guid.NewGuid();
        var place = new Place(ownerId, "House", "", new Coordinates(56, 60), address: "Main street, 1");
        var collection = new PlaceCollection(ownerId, "Architecture");
        collection.AddPlace(ownerId, place.Id);
        collection.ChangeVisibility(ownerId, true);
        var source = new TestReadSource { Places = [place], Collections = [collection] };
        var service = new PublicReadService(source);

        Assert.That(service.GetCollections(), Has.Length.EqualTo(1));
        Assert.That(service.GetCollection(collection.Id), Is.Not.Null);
        Assert.That(service.GetPlace(place.Id), Is.Not.Null);
        Assert.That(service.GetPlace(place.Id)!.Address, Is.EqualTo("Main street, 1"));

        collection.Delete(ownerId);

        Assert.That(service.GetCollections(), Is.Empty);
        Assert.That(service.GetCollection(collection.Id), Is.Null);
        Assert.That(service.GetPlace(place.Id), Is.Null);
        Assert.That(service.GetPlaces(null), Is.Empty);
    }

    [Test]
    public void test_GetCollection_DeletedPlace()
    {
        var authorId = Guid.NewGuid();
        var place = new Place(authorId, "House", "Description", new Coordinates(56, 60));
        var collection = new PlaceCollection(authorId, "Architecture");
        collection.AddPlace(authorId, place.Id);
        collection.ChangeVisibility(authorId, true);
        var source = new TestReadSource { Places = [place], Collections = [collection] };
        var service = new PublicReadService(source);

        place.Delete(authorId);

        Assert.That(service.GetPlace(place.Id), Is.Null);
        Assert.That(service.GetPlaces(null), Is.Empty);
        Assert.That(service.GetPlaces("House"), Is.Empty);

        var response = service.GetCollection(collection.Id)!;
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
        public IReadOnlyList<PlaceCollection> Collections { get; set; } = [];
    }
}
