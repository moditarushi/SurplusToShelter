using Microsoft.EntityFrameworkCore;
using RunAndReason2.API.Models.Entities;

namespace RunAndReason2.API.Data;

public class EFContext : DbContext
{
    public EFContext(DbContextOptions<EFContext> options) : base(options)
    {
    }

    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.StartingDepot)
            .WithMany()
            .HasForeignKey(v => v.StartingDepotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}