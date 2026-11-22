namespace UnderEkat.Api.Contracts;

/// <summary>
/// Карточка подборки с местами и заглушками
/// </summary>
public class CollectionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public CollectionPlaceResponse[] Places { get; set; } = [];
}
