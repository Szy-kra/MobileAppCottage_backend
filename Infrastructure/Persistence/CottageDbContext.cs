using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Domain.Entities;

namespace MobileAppCottage.Infrastructure.Persistence
{
    public class CottageDbContext : IdentityDbContext<User, Role, string>
    {
        public CottageDbContext(DbContextOptions<CottageDbContext> options) : base(options)
        {
        }

        public DbSet<Cottage> Cottages { get; set; }

        // POPRAWIONE: Zmieniono z ICottageImage na CottageImage
        public DbSet<CottageImage> CottageImages { get; set; }

        public DbSet<CottageReservation> CottageReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity
            base.OnModelCreating(modelBuilder);

            // USER
            modelBuilder.Entity<User>(eb =>
            {
                eb.Property(u => u.IsHost)
                  .HasDefaultValue(false)
                  .IsRequired();
            });

            // COTTAGE
            modelBuilder.Entity<Cottage>(eb =>
            {
                eb.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(50);

                eb.Property(c => c.Description)
                  .IsRequired();

                // Pełna konfiguracja ContactDetails
                eb.OwnsOne(c => c.ContactDetails, cd =>
                {
                    cd.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)")
                      .HasColumnName("ContactDetails_Price");

                    cd.Property(p => p.MaxPersons)
                      .HasColumnName("ContactDetails_MaxPersons");

                    cd.Property(p => p.Street)
                      .HasColumnName("ContactDetails_Street");

                    cd.Property(p => p.City)
                      .HasColumnName("ContactDetails_City");

                    cd.Property(p => p.PostalCode)
                      .HasColumnName("ContactDetails_PostalCode");

                    cd.Property(p => p.Description)
                      .HasColumnName("ContactDetails_Description")
                      .IsRequired(false);
                });

                // Relacja z właścicielem
                eb.HasOne(c => c.Owner)
                  .WithMany(u => u.OwnedCottages)
                  .HasForeignKey(c => c.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);

                // Relacja ze zdjęciami
                eb.HasMany(c => c.Images)
                  .WithOne(i => i.Cottage)
                  .HasForeignKey(i => i.CottageId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // RESERVATION
            modelBuilder.Entity<CottageReservation>(eb =>
            {
                eb.HasOne(r => r.ReservedBy)
                  .WithMany(u => u.Reservations)
                  .HasForeignKey(r => r.ReservedById)
                  .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}