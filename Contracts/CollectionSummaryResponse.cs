namespace UnderEkat.Api.Contracts;

/// <summary>
/// Краткие данные подборки для списка
/// </summary>
public class CollectionSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
}
