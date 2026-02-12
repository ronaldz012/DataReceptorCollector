using DataReceptor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataReceptor.Infrastructure.Persistence;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<CarTelemetry>  CarTelemetry { get; set; }
    public DbSet<Car> Cars { get; set; }

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
        });

        modelBuilder.Entity<CarTelemetry>(entity =>
        {
            entity.HasQueryFilter(ct  => ct.DeletedAt == null);
        });


    }
}