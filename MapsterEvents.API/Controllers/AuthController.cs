using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MapsterEvents.Core.DTOs;
using MapsterEvents.Core.Interfaces;

namespace MapsterEvents.API.Controllers
{
    /// <summary>
    /// Kimlik doğrulama controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authService">Kimlik doğrulama servisi</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Kullanıcı kaydı
        /// </summary>
        /// <param name="userRegisterDto">Kullanıcı kayıt DTO'su</param>
        /// <returns>Kayıt sonucu</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(userRegisterDto);
            
            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    user = result.User
                });
            }

            return BadRequest(result.Message);
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        /// <param name="userLoginDto">Kullanıcı giriş DTO'su</param>
        /// <returns>Giriş sonucu</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            Console.WriteLine($"Login request received: Email={userLoginDto?.Email}, ModelState.IsValid={ModelState.IsValid}");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState validation failed:");
                foreach (var modelError in ModelState)
                {
                    Console.WriteLine($"Key: {modelError.Key}, Errors: {string.Join(", ", modelError.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return BadRequest(ModelState);
            }

            Console.WriteLine("Calling AuthService.LoginAsync...");
            var result = await _authService.LoginAsync(userLoginDto);
            Console.WriteLine($"AuthService.LoginAsync returned - Success: {result.Success}, Message: {result.Message}");
            
            if (result.Success)
            {
                Console.WriteLine("Login successful, returning OK response");
                return Ok(new
                {
                    message = result.Message,
                    token = result.Token,
                    user = result.User
                });
            }

            Console.WriteLine($"Login failed, returning BadRequest: {result.Message}");
            return BadRequest(result.Message);
        }

        /// <summary>
        /// E-posta adresi kullanımda mı kontrol eder
        /// </summary>
        /// <param name="email">E-posta adresi</param>
        /// <returns>Kullanım durumu</returns>
        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("E-posta adresi boş olamaz");
            }

            var isInUse = await _authService.IsEmailInUseAsync(email);
            return Ok(isInUse);
        }

        /// <summary>
        /// Token doğrulama endpoint'i (Optimize: Tek çağrı ile doğrulama + kullanıcı bilgisi)
        /// </summary>
        /// <param name="request">Token doğrulama request'i</param>
        /// <returns>Doğrulama sonucu</returns>
        [HttpPost("validate-token")]
        [AllowAnonymous]
        public async Task<ActionResult> ValidateToken([FromBody] TokenValidationRequest request)
        {
            // Performans optimizasyonu: GetUserFromTokenAsync zaten token'ı doğruluyor
            // İki ayrı çağrı yerine tek çağrı ile hem doğrulama hem kullanıcı bilgisi
            var user = await _authService.GetUserFromTokenAsync(request.Token);
            
            if (user != null)
            {
                return Ok(new
                {
                    valid = true,
                    message = "Token geçerli",
                    user = user
                });
            }

            return Ok(new
            {
                valid = false,
                message = "Token geçersiz"
            });
        }

        /// <summary>
        /// Mevcut kullanıcı bilgilerini getirir (Authorize ile)
        /// </summary>
        /// <returns>Kullanıcı bilgileri</returns>
        [HttpGet("me")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            return Ok(new
            {
                message = "Kullanıcı bilgileri",
                user = new
                {
                    id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
                    email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                }
            });
        }
    }

    /// <summary>
    /// Token doğrulama request modeli
    /// </summary>
    public class TokenValidationRequest
    {
        /// <summary>
        /// Doğrulanacak JWT token
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}