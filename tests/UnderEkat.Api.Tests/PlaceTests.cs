using NUnit.Framework;
using UnderEkat.Api.Domain.Entities;
using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Tests;

public class PlaceTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void test_Create_EmptyName(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            new Place(Guid.NewGuid(), name!, "", new Coordinates(56, 60)));
    }

    [Test]
    public void test_Create_NoAuthor()
    {
        Assert.Throws<ArgumentException>(() =>
            new Place(Guid.Empty, "House", "", new Coordinates(56, 60)));
    }

    [Test]
    public void test_Update_ByAuthor()
    {
        var authorId = Guid.NewGuid();
        var place = new Place(authorId, "Old name", "", new Coordinates(56, 60));

        place.Update(authorId, "New name", "New description", new Coordinates(57, 61));

        Assert.That(place.Name, Is.EqualTo("New name"));
        Assert.That(place.Description, Is.EqualTo("New description"));
        Assert.That(place.Location, Is.EqualTo(new Coordinates(57, 61)));
    }

    [Test]
    public void test_Update_ByStranger()
    {
        var place = new Place(Guid.NewGuid(), "House", "", new Coordinates(56, 60));
        var strangerId = Guid.NewGuid();

        Assert.Throws<UnauthorizedAccessException>(() =>
            place.Update(strangerId, "New name", "", new Coordinates(57, 61)));

        Assert.That(place.Name, Is.EqualTo("House"));
    }

    [Test]
    public void test_Delete_ByStranger()
    {
        var place = new Place(Guid.NewGuid(), "House", "", new Coordinates(56, 60));
        var strangerId = Guid.NewGuid();

        Assert.Throws<UnauthorizedAccessException>(() => place.Delete(strangerId));

        Assert.That(place.IsDeleted, Is.False);
    }

    [Test]
    public void test_Update_DeletedPlace()
    {
        var authorId = Guid.NewGuid();
        var place = new Place(authorId, "House", "", new Coordinates(56, 60));

        place.Delete(authorId);

        Assert.That(place.IsDeleted, Is.True);
        Assert.Throws<InvalidOperationException>(() =>
            place.Update(authorId, "New name", "", new Coordinates(57, 61)));
    }
}
