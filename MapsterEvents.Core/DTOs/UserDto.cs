namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Kullanıcı bilgisi DTO (hassas veriler hariç)
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Kullanıcı ID'si
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Kullanıcının tam adı
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// E-posta adresi
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}