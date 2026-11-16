using UnderEkat.Api.Domain.ValueObjects;

namespace UnderEkat.Api.Domain.Entities;

public class Place
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public required Coordinates Location { get; set; }
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
}