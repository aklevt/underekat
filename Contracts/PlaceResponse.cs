namespace UnderEkat.Api.Contracts;

/// <summary>
/// Данные места в ответе API
/// </summary>
public class PlaceResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}