using System.ComponentModel.DataAnnotations;

namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Kategori oluşturma DTO
    /// </summary>
    public class CategoryCreateDto
    {
        /// <summary>
        /// Kategori adı
        /// </summary>
        [Required(ErrorMessage = "Kategori adı gereklidir")]
        [MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Kategori açıklaması
        /// </summary>
        [MaxLength(500, ErrorMessage = "Kategori açıklaması en fazla 500 karakter olabilir")]
        public string Description { get; set; } = string.Empty;
    }
}