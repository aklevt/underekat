using Microsoft.EntityFrameworkCore;
using UnderEkat.Api.Domain.Entities;

namespace UnderEkat.Api.Infrastructure;

/// <summary>
/// Контекст EF Core для чтения и сохранения данных приложения в базе
/// Места хранятся в places, подборки в collections, связи между ними в collection_places
/// </summary>
public class UnderEkatDbContext : DbContext
{
    public DbSet<Place> Places => Set<Place>();
    public DbSet<PlaceCollection> Collections => Set<PlaceCollection>();

    public UnderEkatDbContext(DbContextOptions<UnderEkatDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Вызывается EF Core для настройки таблиц мест, подборок и связей между ними
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurePlaces(modelBuilder);
        ConfigureCollections(modelBuilder);
    }

    /// <summary>
    /// Описывает соответствие свойств Place столбцам таблицы places
    /// Location хранится в столбцах latitude и longitude той же строки
    /// </summary>
    private static void ConfigurePlaces(ModelBuilder modelBuilder)
    {
        var place = modelBuilder.Entity<Place>();

        place.ToTable("places");
        place.HasKey(p => p.Id);
        place.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        place.Property(p => p.AuthorId).HasColumnName("author_id");
        place.Property(p => p.Name).HasColumnName("name").HasMaxLength(65);
        place.Property(p => p.Description).HasColumnName("description").HasMaxLength(1000);
        place.Property(p => p.Address).HasColumnName("address");
        place.Property(p => p.Access).HasColumnName("access");
        place.Property(p => p.Guard).HasColumnName("guard");
        place.Property(p => p.Tags).HasColumnName("tags").HasColumnType("text[]");
        place.Property(p => p.IsDeleted).HasColumnName("is_deleted");

        place.OwnsOne(p => p.Location, location =>
        {
            location.Property(c => c.Latitude).HasColumnName("latitude");
            location.Property(c => c.Longitude).HasColumnName("longitude");
        });
        place.Navigation(p => p.Location).IsRequired();
    }

    /// <summary>
    /// Описывает хранение PlaceCollection в collections, а списка PlaceLinks в collection_places
    /// Изменения связей сохраняются в БД при вызове SaveChanges
    /// Удаление связи из списка исключает место из подборки, но не удаляет само место
    /// </summary>
    private static void ConfigureCollections(ModelBuilder modelkuilder)
    {
        var collection = modelBuilder.Entity<PlaceCollection>();

        collection.ToTable("collections");
        collection.HasKey(c => c.Id);
        collection.Property(c => c.Id).HasColumnName("id").ValueGeneratedNever();
        collection.Property(c => c.OwnerId).HasColumnName("owner_id");
        collection.Property(c => c.Name).HasColumnName("name").HasMaxLength(55);
        collection.Property(c => c.IsPublic).HasColumnName("is_public");
        collection.Property(c => c.IsDeleted).HasColumnName("is_deleted");

        // PlaceIds вычисляется из PlaceLinks и не хранится отдельным столбцом
        collection.Ignore(c => c.PlaceIds);

        // Связь многие ко многим (место может входить в несколько подборок, подборка содержит несколько мест)
        collection.OwnsMany(c => c.PlaceLinks, link =>
        {
            link.ToTable("collection_places");
            link.WithOwner().HasForeignKey(l => l.CollectionId);

            // Каждая пара id подборки и места уникальна (составной первичный ключ) 
            // Запрещает повторное добавление одного места в подборку, но не запрещает добавление одного места в разные подборки
            link.HasKey(l => new { l.CollectionId, l.PlaceId });

            link.Property(l => l.CollectionId).HasColumnName("collection_id").ValueGeneratedNever();
            link.Property(l => l.PlaceId).HasColumnName("place_id").ValueGeneratedNever();
            link.HasOne<Place>().WithMany().HasForeignKey(l => l.PlaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        collection.Navigation(c => c.PlaceLinks).HasField("placeLinks");
    }
}
