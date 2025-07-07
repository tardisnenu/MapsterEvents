using System.Net;
using System.Text.Json;

namespace MapsterEvents.API.Middleware
{
    /// <summary>
    /// Global hata yakalama middleware
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Sonraki middleware</param>
        /// <param name="logger">Logger</param>
        /// <param name="environment">Host environment (production güvenliği için)</param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Middleware işlemi
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Hata işleme (Production güvenli + HTTP semantik doğru)
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">Hata</param>
        /// <returns>Task</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Production güvenliği: Geliştirme ortamında detaylı, production'da jenerik mesaj
            var errorDetails = _environment.IsDevelopment() 
                ? exception.Message 
                : "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";

            var response = new
            {
                message = "Bir hata oluştu",
                details = errorDetails,
                statusCode = (int)HttpStatusCode.InternalServerError
            };

            // HTTP semantik doğru status code mapping
            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Forbidden, // 403: Kimlik doğrulandı ama yetki yok
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            response = response with { statusCode = context.Response.StatusCode };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}