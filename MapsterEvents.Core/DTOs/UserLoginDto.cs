using System.ComponentModel.DataAnnotations;

namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Kullanıcı giriş DTO
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// E-posta adresi
        /// </summary>
        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [MaxLength(255, ErrorMessage = "E-posta adresi en fazla 255 karakter olabilir")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Şifre
        /// </summary>
        [Required(ErrorMessage = "Şifre gereklidir")]
        public string Password { get; set; } = string.Empty;
    }
}