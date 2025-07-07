using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MapsterEvents.Core.Interfaces;

namespace MapsterEvents.Core.Entities
{
    /// <summary>
    /// Etkinlik kayıt entity sınıfı
    /// </summary>
    public class Registration : BaseEntity, IRegistrableEntity
    {
        /// <summary>
        /// Etkinlik ID'si
        /// </summary>
        [Required]
        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        
        /// <summary>
        /// Kullanıcı ID'si
        /// </summary>
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        
        /// <summary>
        /// Kayıt tarihi (SaveChanges sırasında otomatik atanır)
        /// </summary>
        [Required]
        public DateTime RegistrationDate { get; set; }
        
        /// <summary>
        /// Etkinlik bilgisi
        /// </summary>
        [InverseProperty(nameof(Event.Registrations))]
        public virtual Event Event { get; set; } = null!;
        
        /// <summary>
        /// Kullanıcı bilgisi
        /// </summary>
        [InverseProperty(nameof(User.Registrations))]
        public virtual User User { get; set; } = null!;
    }
}