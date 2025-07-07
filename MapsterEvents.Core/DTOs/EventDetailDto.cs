namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Etkinlik detayı için DTO
    /// </summary>
    public class EventDetailDto
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
        /// Etkinlik açıklaması
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
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
        /// Organizatör e-posta
        /// </summary>
        public string OrganizerEmail { get; set; } = string.Empty;
        
        /// <summary>
        /// Katılımcı listesi
        /// </summary>
        public List<ParticipantDto> Participants { get; set; } = new();
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
    
    /// <summary>
    /// Katılımcı DTO
    /// </summary>
    public class ParticipantDto
    {
        /// <summary>
        /// Kullanıcı ID'si
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// Kullanıcı adı
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// E-posta adresi
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Kayıt tarihi
        /// </summary>
        public DateTime RegistrationDate { get; set; }
    }
}