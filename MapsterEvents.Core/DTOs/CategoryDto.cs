namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Kategori detay DTO (Sadece okuma amaçlı)
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// Kategori ID'si
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Kategori adı
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Kategori açıklaması
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Bu kategoriye ait etkinlik sayısı
        /// </summary>
        public int EventCount { get; set; }
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}