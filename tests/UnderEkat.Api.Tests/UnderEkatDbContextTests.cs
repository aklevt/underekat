using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UnderEkat.Api.Domain.Entities;
using UnderEkat.Api.Domain.ValueObjects;
using UnderEkat.Api.Infrastructure;

namespace UnderEkat.Api.Tests;

public class UnderEkatDbContextTests
{
    [Test]
    public void test_ChangePlaces_Tracked()
    {
        var options = new DbContextOptionsBuilder<UnderEkatDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused")
            .Options;
        using var db = new UnderEkatDbContext(options);
        var ownerId = Guid.NewGuid();
        var place = new Place(ownerId, "House", "", new Coordinates(56, 60));
        var collection = new Collection(ownerId, "Architecture");
        collection.AddPlace(ownerId, place.Id);
        var oldLink = collection.PlaceLinks.Single();
        db.Attach(place);
        db.Attach(collection);

        collection.RemovePlace(ownerId, place.Id);
        var nextPlaceId = Guid.NewGuid();
        collection.AddPlace(ownerId, nextPlaceId);
        db.ChangeTracker.DetectChanges();

        Assert.That(db.Entry(oldLink).State, Is.EqualTo(EntityState.Deleted));
        Assert.That(db.Entry(collection.PlaceLinks.Single()).State, Is.EqualTo(EntityState.Added));
        Assert.That(db.Entry(place).State, Is.EqualTo(EntityState.Unchanged));
    }
}
