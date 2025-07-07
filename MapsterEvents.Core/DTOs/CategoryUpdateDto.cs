using System.ComponentModel.DataAnnotations;

namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Kategori güncelleme DTO
    /// </summary>
    public class CategoryUpdateDto
    {
        /// <summary>
        /// Kategori ID'si
        /// </summary>
        [Required(ErrorMessage = "Kategori ID'si gereklidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Kategori ID'si pozitif bir sayı olmalıdır")]
        public int Id { get; set; }
        
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