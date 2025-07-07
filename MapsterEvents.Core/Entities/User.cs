using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapsterEvents.Core.Entities
{
    /// <summary>
    /// Kullanıcı entity sınıfı
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Kullanıcının tam adı
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// E-posta adresi
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Şifre hash değeri
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        /// <summary>
        /// Şifre salt değeri
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string PasswordSalt { get; set; } = string.Empty;
        
        /// <summary>
        /// Kullanıcının organize ettiği etkinlikler
        /// </summary>
        [InverseProperty(nameof(Event.Organizer))]
        public virtual ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        
        /// <summary>
        /// Kullanıcının katıldığı etkinlik kayıtları
        /// </summary>
        [InverseProperty(nameof(Registration.User))]
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}