using System.ComponentModel.DataAnnotations;
using MapsterEvents.Core.DTOs.Attributes;

namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Yeni etkinlik oluşturma DTO
    /// </summary>
    public class EventCreateDto
    {
        /// <summary>
        /// Etkinlik başlığı
        /// </summary>
        [Required(ErrorMessage = "Etkinlik başlığı gereklidir")]
        [MaxLength(200, ErrorMessage = "Etkinlik başlığı en fazla 200 karakter olabilir")]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik açıklaması
        /// </summary>
        [Required(ErrorMessage = "Etkinlik açıklaması gereklidir")]
        [MaxLength(1000, ErrorMessage = "Etkinlik açıklaması en fazla 1000 karakter olabilir")]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik tarihi
        /// </summary>
        [Required(ErrorMessage = "Etkinlik tarihi gereklidir")]
        [FutureDate(minimumMinutesFromNow: 30)] // Etkinlik en az 30 dakika sonrası olmalı
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Etkinlik konumu
        /// </summary>
        [Required(ErrorMessage = "Etkinlik konumu gereklidir")]
        [MaxLength(300, ErrorMessage = "Etkinlik konumu en fazla 300 karakter olabilir")]
        public string Location { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik resmi URL'si
        /// </summary>
        [MaxLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir")]
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Kategori ID'si
        /// </summary>
        [Required(ErrorMessage = "Kategori seçimi gereklidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçmelisiniz")]
        public int CategoryId { get; set; }
    }
}