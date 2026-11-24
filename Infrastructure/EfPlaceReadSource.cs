using Microsoft.EntityFrameworkCore;
using UnderEkat.Api.Application;
using UnderEkat.Api.Domain.Entities;

namespace UnderEkat.Api.Infrastructure;

/// <summary>
/// Возвращает места и подборки из БД через EF Core
/// </summary>
public class EfPlaceReadSource : IPlaceReadSource
{
    private readonly UnderEkatDbContext db;

    public EfPlaceReadSource(UnderEkatDbContext db)
    {
        this.db = db;
    }

    public async Task<IReadOnlyList<Place>> GetPlacesAsync(IReadOnlyCollection<Guid>? ids = null)
    {
        var query = db.Places.AsNoTracking();

        if (ids != null)
            query = query.Where(p => ids.Contains(p.Id));

        return await query.OrderBy(p => p.Name).ThenBy(p => p.Id).ToListAsync();
    }

    public async Task<IReadOnlyList<Collection>> GetCollectionsAsync()
    {
        return await db.Collections.AsNoTracking()
            .OrderBy(c => c.Name).ThenBy(c => c.Id).ToListAsync();
    }
}
