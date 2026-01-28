using Entities.Concrete;
using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework;

public class SmartCarContext : DbContext
{
    public SmartCarContext()
    {
    }

    public SmartCarContext(DbContextOptions<SmartCarContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback for non-DI usage (e.g. Generic Repository internal instantiation)
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SmartCarDb;Username=postgres;Password=1955");
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleImage> VehicleImages { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<AdditionalService> AdditionalServices { get; set; }
    public DbSet<ReservationService> ReservationServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicit Primary Keys
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Branch>().HasKey(b => b.BranchId);
        modelBuilder.Entity<VehicleModel>().HasKey(v => v.ModelId);
        modelBuilder.Entity<Vehicle>().HasKey(v => v.VehicleId);
        modelBuilder.Entity<Reservation>().HasKey(r => r.ReservationId);
        modelBuilder.Entity<Rental>().HasKey(r => r.RentalId);
        modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
        modelBuilder.Entity<Insurance>().HasKey(i => i.InsuranceId);
        modelBuilder.Entity<AdditionalService>().HasKey(a => a.ServiceId);

        // TPT Inheritance Strategy
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Employee>().ToTable("Employees");

        // Maintenance: Weak Entity (Composite Key)
        modelBuilder.Entity<Maintenance>()
            .HasKey(m => new { m.VehicleId, m.MaintenanceId });
        
        modelBuilder.Entity<Maintenance>()
            .HasOne(m => m.Vehicle)
            .WithMany()
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleImages Cascade configuration removed to fix 42703 error
        // Will handle deletion logic in Manager if needed.

        // ReservationService: Many-to-Many Junction
        modelBuilder.Entity<ReservationService>()
            .HasKey(rs => new { rs.ReservationId, rs.ServiceId });

        modelBuilder.Entity<ReservationService>()
            .HasOne(rs => rs.Reservation)
            .WithMany()
            .HasForeignKey(rs => rs.ReservationId);

        modelBuilder.Entity<ReservationService>()
            .HasOne(rs => rs.Service)
            .WithMany()
            .HasForeignKey(rs => rs.ServiceId);

        // Rental -> Payment (One-to-One)
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Payment)
            .WithOne(p => p.Rental)
            .HasForeignKey<Payment>(p => p.RentalId);

        // Vehicle -> Insurance (One-to-One)
        modelBuilder.Entity<Insurance>()
            .HasOne(i => i.Vehicle)
            .WithOne() 
            .HasForeignKey<Insurance>(i => i.VehicleId);
    }
}
