using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CottageAPI.Entities
{
    // IdentityDbContext tworzy tabele AspNetUsers i AspNetRoles [cite: 2026-01-12]
    public class CottageDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public CottageDbContext(DbContextOptions<CottageDbContext> options) : base(options)
        {
        }

        public DbSet<Cottage> Cottages { get; set; } = null!;
        public DbSet<CottageImage> CottageImages { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Buduje tabele systemowe Identity [cite: 2026-01-12]
            base.OnModelCreating(modelBuilder);

            // KONFIGURACJA USER (Tabela: AspNetUsers)
            modelBuilder.Entity<User>(eb =>
            {
                eb.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
                eb.Property(u => u.LastName).HasMaxLength(50).IsRequired();
            });

            // KONFIGURACJA COTTAGE
            modelBuilder.Entity<Cottage>(eb =>
            {
                eb.Property(c => c.Name).IsRequired().HasMaxLength(50);
                eb.Property(c => c.Price).HasPrecision(18, 2);
                eb.Property(c => c.Description).HasMaxLength(100);
                eb.Property(c => c.About).HasMaxLength(1000);

                // Pola adresowe
                eb.Property(c => c.City).IsRequired().HasMaxLength(50);
                eb.Property(c => c.Street).IsRequired().HasMaxLength(100);
                eb.Property(c => c.PostalCode).IsRequired().HasMaxLength(10);



            });

            // KONFIGURACJA RESERVATION
            modelBuilder.Entity<Reservation>(eb =>
            {
                eb.Property(r => r.PricePerNightSnapshot).HasPrecision(18, 2);
                eb.Property(r => r.TotalPrice).HasPrecision(18, 2);

                // Zabezpieczenie przed kaskadowym usuwaniem (SQL Server Error 1785)
                eb.HasOne(r => r.Cottage)
                  .WithMany(c => c.Reservations)
                  .HasForeignKey(r => r.CottageId)
                  .OnDelete(DeleteBehavior.NoAction);
            });

            // KONFIGURACJA COTTAGEIMAGE
            modelBuilder.Entity<CottageImage>(eb =>
            {
                eb.Property(i => i.Url).IsRequired();

                eb.HasOne(i => i.Cottage)
                  .WithMany(c => c.Images)
                  .HasForeignKey(i => i.CottageId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}