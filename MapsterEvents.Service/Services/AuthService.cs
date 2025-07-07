using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Mapster;
using MapsterEvents.Core.DTOs;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Repository.Data;

namespace MapsterEvents.Service.Services
{
    /// <summary>
    /// Kimlik doğrulama servis implementasyonu
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly MapsterEventsDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepository">Kullanıcı repository</param>
        /// <param name="configuration">Yapılandırma</param>
        /// <param name="passwordHasher">Şifre hash'leme servisi</param>
        /// <param name="context">Veritabanı context (Unit of Work için)</param>
        public AuthService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasher<User> passwordHasher, MapsterEventsDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _context = context;
        }

        /// <summary>
        /// Kullanıcı kaydı
        /// </summary>
        /// <param name="userRegisterDto">Kullanıcı kayıt DTO'su</param>
        /// <returns>Kayıt sonucu</returns>
        public async Task<(bool Success, string Message, UserDto? User)> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            try
            {
                // E-posta adresi kontrolü
                var isEmailInUse = await _userRepository.IsEmailInUseAsync(userRegisterDto.Email);
                if (isEmailInUse)
                {
                    return (false, "Bu e-posta adresi zaten kullanılıyor", null);
                }

                // Kullanıcı oluşturma
                var user = userRegisterDto.Adapt<User>();
                
                // ASP.NET Core Identity PasswordHasher kullanarak güvenli hash
                user.PasswordHash = _passwordHasher.HashPassword(user, userRegisterDto.Password);
                user.PasswordSalt = string.Empty; // Modern approach: Salt artık hash içinde gömülü

                var createdUser = await _userRepository.AddAsync(user);
                
                // Unit of Work pattern - Tek transaction içinde save
                await _context.SaveChangesAsync();
                
                var userDto = createdUser.Adapt<UserDto>();

                return (true, "Kayıt başarılı", userDto);
            }
            catch (Exception ex)
            {
                return (false, $"Kayıt sırasında hata oluştu: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        /// <param name="userLoginDto">Kullanıcı giriş DTO'su</param>
        /// <returns>Giriş sonucu</returns>
        public async Task<(bool Success, string Message, string Token, UserDto? User)> LoginAsync(UserLoginDto userLoginDto)
        {
            try
            {
                Console.WriteLine($"AuthService.LoginAsync started - Email: {userLoginDto.Email}");
                
                // Kullanıcı kontrolü
                Console.WriteLine("Searching for user by email...");
                var user = await _userRepository.GetUserByEmailAsync(userLoginDto.Email);
                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return (false, "Geçersiz e-posta adresi veya şifre", string.Empty, null);
                }
                Console.WriteLine($"User found - ID: {user.Id}, Email: {user.Email}");

                // Şifre kontrolü - ASP.NET Core Identity PasswordHasher kullanarak
                Console.WriteLine("Verifying password...");
                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLoginDto.Password);
                Console.WriteLine($"Password verification result: {verificationResult}");
                
                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    Console.WriteLine("Password verification failed");
                    return (false, "Geçersiz e-posta adresi veya şifre", string.Empty, null);
                }

                // Token oluşturma
                Console.WriteLine("Creating user DTO...");
                var userDto = user.Adapt<UserDto>();
                Console.WriteLine($"UserDto created - ID: {userDto.Id}, Email: {userDto.Email}");
                
                Console.WriteLine("Generating JWT token...");
                var token = GenerateJwtToken(userDto);
                Console.WriteLine($"JWT token generated - Length: {token.Length}");

                Console.WriteLine("Login successful, returning result");
                return (true, "Giriş başarılı", token, userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService.LoginAsync Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return (false, $"Giriş sırasında hata oluştu: {ex.Message}", string.Empty, null);
            }
        }

        /// <summary>
        /// JWT token oluşturur (Internal implementation)
        /// </summary>
        /// <param name="user">Kullanıcı</param>
        /// <returns>JWT token</returns>
        private string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "MapsterEventsSecretKey123456789");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "MapsterEventsAPI",
                Audience = _configuration["Jwt:Audience"] ?? "MapsterEventsClient"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Şifre hash'ler (Internal implementation) - ASP.NET Core Identity kullanarak
        /// </summary>
        /// <param name="user">Kullanıcı</param>
        /// <param name="password">Şifre</param>
        /// <returns>Hash</returns>
        private string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        /// <summary>
        /// Şifre doğrular (Internal implementation) - ASP.NET Core Identity kullanarak
        /// </summary>
        /// <param name="user">Kullanıcı</param>
        /// <param name="hashedPassword">Hash'lenmiş şifre</param>
        /// <param name="providedPassword">Girilen şifre</param>
        /// <returns>Doğrulama sonucu</returns>
        private PasswordVerificationResult VerifyPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }

        /// <summary>
        /// E-posta adresi kullanımda mı kontrol eder
        /// </summary>
        /// <param name="email">E-posta adresi</param>
        /// <returns>Kullanımda mı</returns>
        public async Task<bool> IsEmailInUseAsync(string email)
        {
            return await _userRepository.IsEmailInUseAsync(email);
        }

        /// <summary>
        /// Kullanıcı token'ının geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Token geçerli mi</returns>
        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return Task.FromResult(false);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "MapsterEventsSecretKey123456789");

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"] ?? "MapsterEventsAPI",
                    ValidAudience = _configuration["Jwt:Audience"] ?? "MapsterEventsClient",
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Token'dan kullanıcı bilgilerini çıkarır
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Kullanıcı bilgileri</returns>
        public async Task<UserDto?> GetUserFromTokenAsync(string token)
        {
            try
            {
                if (!await ValidateTokenAsync(token))
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                    return null;

                var user = await _userRepository.GetByIdAsync(userId);
                return user?.Adapt<UserDto>();
            }
            catch
            {
                return null;
            }
        }
    }
}