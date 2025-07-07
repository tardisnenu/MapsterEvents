using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapsterEvents.Core.Entities
{
    /// <summary>
    /// Etkinlik kategorisi entity sınıfı
    /// </summary>
    public class Category : BaseEntity
    {
        /// <summary>
        /// Kategori adı
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Kategori açıklaması
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Bu kategoriye ait etkinlikler
        /// </summary>
        [InverseProperty(nameof(Event.Category))]
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}