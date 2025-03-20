using HQB.WebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HQB.WebApi.Data;
public class ZorgAppDbContext : IdentityDbContext<OuderVoogd>
{
    public ZorgAppDbContext(DbContextOptions<ZorgAppDbContext> options) : base(options) { }

    public DbSet<Arts> Artsen { get; set; }
    public DbSet<Patient> Patienten { get; set; }
    public DbSet<Traject> Trajecten { get; set; }
    public DbSet<TrajectZorgMoment> TrajectZorgMomenten { get; set; }
    public DbSet<ZorgMoment> ZorgMomenten { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Arts>(entity =>
        {
            entity.HasKey(a => a.ID);
            entity.HasIndex(a => new { a.Naam, a.Specialisatie }).IsUnique();
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.ID);

            entity.HasOne(p => p.OuderVoogd)
                .WithMany() // No need to configure inverse navigation
                .HasForeignKey(p => p.OuderVoogdId)
                .IsRequired() // Ensures a patient must have an OuderVoogd
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of OuderVoogd if referenced by Patient

            entity.HasOne(p => p.Arts)
                .WithMany(a => a.Patients) // Ensure Arts has a navigation property
                .HasForeignKey(p => p.ArtsID)
                .IsRequired(false) // Allow null values (optional doctor)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Traject)
                .WithMany()
                .HasForeignKey(p => p.TrajectID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Traject>(entity =>
        {
            entity.HasKey(t => t.ID);
            entity.HasIndex(t => t.Naam).IsUnique();
        });

        modelBuilder.Entity<TrajectZorgMoment>(entity =>
        {
            entity.HasKey(tzm => new { tzm.TrajectID, tzm.ZorgMomentID });
            entity.HasOne(tzm => tzm.Traject)
                .WithMany()
                .HasForeignKey(tzm => tzm.TrajectID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(tzm => tzm.ZorgMoment)
                .WithMany()
                .HasForeignKey(tzm => tzm.ZorgMomentID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ZorgMoment>(entity =>
        {
            entity.HasKey(zm => zm.ID);
            entity.HasIndex(zm => zm.Naam).IsUnique();
        });
    }
}
