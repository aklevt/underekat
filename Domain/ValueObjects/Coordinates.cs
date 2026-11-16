namespace UnderEkat.Api.Domain.ValueObjects;

public sealed record Coordinates
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordinates(double latitude, double longitude)
    {
        if (!(latitude >= -90 && latitude <= 90) || !(longitude >= -180 && longitude <= 180))
            throw new ArgumentOutOfRangeException(
                nameof(Coordinates),
                "Координаты должны быть в диапазоне: широта от -90 до 90 градусов, долгота от -180 до 180 градусов"
            );

        Latitude = latitude;
        Longitude = longitude;
    }
}
