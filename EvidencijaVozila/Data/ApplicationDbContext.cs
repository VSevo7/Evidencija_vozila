using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<OrganizationalUnit> OrganizationalUnits => Set<OrganizationalUnit>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<ServiceDepartment> ServiceDepartments => Set<ServiceDepartment>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleOrder> VehicleOrders => Set<VehicleOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasIndex(x => x.Username)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(x => x.RegistrationNumber)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .Property(x => x.PurchasePrice)
            .HasPrecision(12, 2);

        modelBuilder.Entity<VehicleOrder>()
            .HasIndex(x => x.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.OrganizationalUnit)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.OrganizationalUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Sector)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.ServiceDepartment)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.ServiceDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VehicleOrder>()
            .HasOne(x => x.Driver)
            .WithMany(x => x.DrivenOrders)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VehicleOrder>()
            .HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedOrders)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VehicleOrder>()
            .HasOne(x => x.OrganizationalUnit)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.OrganizationalUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
