using MapsterEvents.Core.Interfaces;

namespace MapsterEvents.Core.Entities
{
    /// <summary>
    /// Tüm entity sınıfları için temel sınıf
    /// </summary>
    public abstract class BaseEntity : IAuditableEntity
    {
        /// <summary>
        /// Birincil anahtar
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Oluşturulma tarihi (SaveChanges sırasında otomatik atanır)
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Son güncellenme tarihi (SaveChanges sırasında otomatik güncellenir)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}