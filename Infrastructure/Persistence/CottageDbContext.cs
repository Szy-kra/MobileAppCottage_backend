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
        public DbSet<CottageImage> CottageImages { get; set; }
        public DbSet<CottageReservation> CottageReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bardzo ważne: wywołanie base konfiguruje tabele Identity (AspNetUsers, AspNetRoles itd.)
            base.OnModelCreating(modelBuilder);

            // Konfiguracja Użytkownika - obsługa IsHost
            modelBuilder.Entity<User>(eb =>
            {
                // To ustawienie sprawi, że SQL nie odrzuci zapisu, jeśli IsHost nie zostanie podane
                eb.Property(u => u.IsHost)
                  .HasDefaultValue(false)
                  .IsRequired();
            });

            modelBuilder.Entity<Cottage>(eb =>
            {
                eb.Property(c => c.Name).IsRequired().HasMaxLength(50);

                // Description jako wymagane
                eb.Property(c => c.Description).IsRequired();

                eb.OwnsOne(c => c.ContactDetails, cd =>
                {
                    cd.Property(p => p.Price).HasColumnType("decimal(18,2)");
                    // Opcjonalne opisy wewnątrz detali
                    cd.Property(p => p.Description).IsRequired(false);
                });

                // Relacja: Domek ma jednego właściciela (User), Właściciel może mieć wiele domków
                eb.HasOne(c => c.Owner)
                  .WithMany(u => u.OwnedCottages)
                  .HasForeignKey(c => c.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade); // Usunięcie usera usuwa jego domki
            });

            modelBuilder.Entity<CottageReservation>(eb =>
            {
                // Relacja: Rezerwacja przypisana do usera (Gosc)
                eb.HasOne(r => r.ReservedBy)
                  .WithMany(u => u.Reservations)
                  .HasForeignKey(r => r.ReservedById)
                  .OnDelete(DeleteBehavior.Restrict); // Nie usuwamy rezerwacji przy usunięciu usera (bezpieczniej)
            });
        }
    }
}