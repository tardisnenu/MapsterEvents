namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Etkinlik listesi için özet DTO
    /// </summary>
    public class EventListItemDto
    {
        /// <summary>
        /// Etkinlik ID'si
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Etkinlik başlığı
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik açıklaması özeti (Liste görünümü için kısaltılmış)
        /// </summary>
        public string ShortDescription { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik tarihi
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Etkinlik konumu
        /// </summary>
        public string Location { get; set; } = string.Empty;
        
        /// <summary>
        /// Etkinlik resmi URL'si
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Kategori adı
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
        
        /// <summary>
        /// Organizatör adı
        /// </summary>
        public string OrganizerName { get; set; } = string.Empty;
        
        /// <summary>
        /// Katılımcı sayısı
        /// </summary>
        public int ParticipantCount { get; set; }
        
        /// <summary>
        /// Etkinlik durumu (Yaklaşan/Geçmiş)
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}