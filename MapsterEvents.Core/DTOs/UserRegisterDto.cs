using System.ComponentModel.DataAnnotations;

namespace MapsterEvents.Core.DTOs
{
    /// <summary>
    /// Kullanıcı kayıt DTO
    /// </summary>
    public class UserRegisterDto
    {
        /// <summary>
        /// Kullanıcının tam adı
        /// </summary>
        [Required(ErrorMessage = "Tam ad gereklidir")]
        [MaxLength(100, ErrorMessage = "Tam ad en fazla 100 karakter olabilir")]
        public string FullName { get; set; } = string.Empty;
        
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
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [MaxLength(100, ErrorMessage = "Şifre en fazla 100 karakter olabilir")]
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// Şifre tekrarı
        /// </summary>
        [Required(ErrorMessage = "Şifre tekrarı gereklidir")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}