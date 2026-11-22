using NUnit.Framework;
using UnderEkat.Api.Domain.Entities;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Tests;

public class PlaceCollectionTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void test_Create_EmptyName(string? name)
    {
        Assert.Throws<ArgumentException>(() => new PlaceCollection(Guid.NewGuid(), name!));
    }

    [Test]
    public void test_Create_NoOwner()
    {
        Assert.Throws<ArgumentException>(() => new PlaceCollection(Guid.Empty, "Architecture"));
    }

    [Test]
    public void test_AddPlace_Twice()
    {
        var ownerId = Guid.NewGuid();
        var collection = new PlaceCollection(ownerId, "Architecture");
        var placeId = Guid.NewGuid();

        collection.AddPlace(ownerId, placeId);
        collection.AddPlace(ownerId, placeId);

        Assert.That(collection.PlaceIds, Is.EqualTo(new[] { placeId }));
    }

    [Test]
    public void test_Change_ByStranger()
    {
        var ownerId = Guid.NewGuid();
        var collection = new PlaceCollection(ownerId, "Architecture");
        var placeId = Guid.NewGuid();
        collection.AddPlace(ownerId, placeId);
        var strangerId = Guid.NewGuid();

        Assert.Throws<UnauthorizedAccessException>(() => collection.AddPlace(strangerId, Guid.NewGuid()));
        Assert.Throws<UnauthorizedAccessException>(() => collection.RemovePlace(strangerId, placeId));
        Assert.Throws<UnauthorizedAccessException>(() => collection.ChangeVisibility(strangerId, true));
        Assert.Throws<UnauthorizedAccessException>(() => collection.Delete(strangerId));
    }

    [Test]
    public void test_RemovePlace_OneCollection()
    {
        var ownerId = Guid.NewGuid();
        var place = new Place(Guid.NewGuid(), "House", "", new Coordinates(56, 60));
        var first = new PlaceCollection(ownerId, "Architecture");
        var second = new PlaceCollection(ownerId, "Walk");
        first.AddPlace(ownerId, place.Id);
        second.AddPlace(ownerId, place.Id);

        first.RemovePlace(ownerId, place.Id);

        Assert.That(first.PlaceIds, Is.Empty);
        Assert.That(second.PlaceIds, Does.Contain(place.Id));
        Assert.That(place.IsDeleted, Is.False);
    }

    [Test]
    public void test_Delete_WithPlace()
    {
        var ownerId = Guid.NewGuid();
        var place = new Place(Guid.NewGuid(), "House", "", new Coordinates(56, 60));
        var collection = new PlaceCollection(ownerId, "Architecture");
        collection.AddPlace(ownerId, place.Id);

        collection.Delete(ownerId);

        Assert.That(collection.IsDeleted, Is.True);
        Assert.That(place.IsDeleted, Is.False);
    }
}
