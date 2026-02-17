using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<CarTelemetry>  CarTelemetry { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<CarSnapshot> CarSnapshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasMany(c  => c.CarTelemetries)
                .WithOne(c => c.Car)
                .HasForeignKey(c => c.CarId);
            
            entity.HasIndex(u => u.Name)
                .IsUnique();
            
            entity.HasOne(c => c.CarSnapshot)
                .WithOne(c => c.Car)
                .HasForeignKey<CarSnapshot>(c => c.CarId);
        });

        modelBuilder.Entity<CarTelemetry>(entity =>
        {
            entity.HasQueryFilter(ct  => ct.DeletedAt == null);
        });


    }
}