using System.Text.Json.Serialization;

namespace UnderEkat.Api.Contracts;

/// <summary>
/// Ссылка на место подборки, для удаленного места содержит только id и признак удаления
/// </summary>
public class CollectionPlaceResponse
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PlaceResponse? Place { get; set; }
}
