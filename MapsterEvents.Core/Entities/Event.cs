using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapsterEvents.Core.Entities
{
    /// <summary>
    /// Etkinlik entity sınıfı
    /// </summary>
    public class Event : BaseEntity
    {
        /// <summary>
        /// Etkinlik başlığı
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik açıklaması
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik tarihi
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Etkinlik konumu
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Location { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik resmi URL'si
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Kategori ID'si
        /// </summary>
        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        
        /// <summary>
        /// Organizatör ID'si
        /// </summary>
        [Required]
        [ForeignKey(nameof(Organizer))]
        public int OrganizerId { get; set; }
        
        /// <summary>
        /// Etkinlik kategorisi
        /// </summary>
        [InverseProperty(nameof(Category.Events))]
        public virtual Category Category { get; set; } = null!;
        
        /// <summary>
        /// Etkinlik organizatörü
        /// </summary>
        [InverseProperty(nameof(User.OrganizedEvents))]
        public virtual User Organizer { get; set; } = null!;
        
        /// <summary>
        /// Etkinlik kayıtları
        /// </summary>
        [InverseProperty(nameof(Registration.Event))]
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}