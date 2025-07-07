using Microsoft.EntityFrameworkCore;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;

namespace MapsterEvents.Repository.Data
{
    /// <summary>
    /// MapsterEvents veritabanı context sınıfı
    /// </summary>
    public class MapsterEventsDbContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">DbContext seçenekleri</param>
        public MapsterEventsDbContext(DbContextOptions<MapsterEventsDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Etkinlikler tablosu
        /// </summary>
        public DbSet<Event> Events { get; set; } = null!;
        
        /// <summary>
        /// Kategoriler tablosu
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;
        
        /// <summary>
        /// Kullanıcılar tablosu
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;
        
        /// <summary>
        /// Kayıtlar tablosu
        /// </summary>
        public DbSet<Registration> Registrations { get; set; } = null!;

        /// <summary>
        /// Model yapılandırması
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Event yapılandırması (sadece karmaşık kurallar)
            modelBuilder.Entity<Event>(entity =>
            {
                // İlişkiler ve cascade behavior
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Events)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Organizer)
                    .WithMany(u => u.OrganizedEvents)
                    .HasForeignKey(e => e.OrganizerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category yapılandırması (sadece unique constraint)
            modelBuilder.Entity<Category>(entity =>
            {
                // Unique constraint - business rule
                entity.HasIndex(c => c.Name).IsUnique();
            });

            // User yapılandırması (sadece unique constraint)
            modelBuilder.Entity<User>(entity =>
            {
                // Unique constraint - business rule
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Registration yapılandırması (sadece karmaşık kurallar)
            modelBuilder.Entity<Registration>(entity =>
            {
                // İlişkiler ve cascade behavior
                entity.HasOne(r => r.Event)
                    .WithMany(e => e.Registrations)
                    .HasForeignKey(r => r.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(r => r.User)
                    .WithMany(u => u.Registrations)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Unique constraint - business rule
                entity.HasIndex(r => new { r.EventId, r.UserId }).IsUnique();
            });
        }

        /// <summary>
        /// SaveChanges override - Audit trail implementasyonu
        /// </summary>
        /// <returns>Etkilenen kayıt sayısı</returns>
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        /// <summary>
        /// SaveChangesAsync override - Audit trail implementasyonu
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Etkilenen kayıt sayısı</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Audit alanlarını günceller
        /// </summary>
        private void UpdateAuditFields()
        {
            var utcNow = DateTime.UtcNow;

            // Standard audit fields (CreatedAt, UpdatedAt)
            var auditableEntries = ChangeTracker.Entries<IAuditableEntity>();
            foreach (var entry in auditableEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = utcNow;
                        entry.Entity.UpdatedAt = null;
                        break;

                    case EntityState.Modified:
                        // CreatedAt değiştirilmesini engelle
                        entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                        entry.Entity.UpdatedAt = utcNow;
                        break;
                }
            }

            // Registration-specific audit fields (RegistrationDate)
            var registrableEntries = ChangeTracker.Entries<IRegistrableEntity>();
            foreach (var entry in registrableEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.RegistrationDate = utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // RegistrationDate değiştirilmesini engelle - immutable
                    entry.Property(nameof(IRegistrableEntity.RegistrationDate)).IsModified = false;
                }
            }
        }
    }
}