using MapsterEvents.Core.DTOs;

namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Kimlik doğrulama servis interface - Public contract only
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Kullanıcı kaydı
        /// </summary>
        /// <param name="userRegisterDto">Kullanıcı kayıt DTO'su</param>
        /// <returns>Kayıt sonucu</returns>
        Task<(bool Success, string Message, UserDto? User)> RegisterAsync(UserRegisterDto userRegisterDto);
        
        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        /// <param name="userLoginDto">Kullanıcı giriş DTO'su</param>
        /// <returns>Giriş sonucu</returns>
        Task<(bool Success, string Message, string Token, UserDto? User)> LoginAsync(UserLoginDto userLoginDto);
        
        /// <summary>
        /// E-posta adresi kullanımda mı kontrol eder
        /// </summary>
        /// <param name="email">E-posta adresi</param>
        /// <returns>Kullanımda mı</returns>
        Task<bool> IsEmailInUseAsync(string email);

        /// <summary>
        /// Kullanıcı token'ının geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Token geçerli mi</returns>
        Task<bool> ValidateTokenAsync(string token);

        /// <summary>
        /// Token'dan kullanıcı bilgilerini çıkarır
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Kullanıcı bilgileri</returns>
        Task<UserDto?> GetUserFromTokenAsync(string token);
    }
}