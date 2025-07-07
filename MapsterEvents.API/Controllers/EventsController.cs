using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MapsterEvents.Core.DTOs;
using MapsterEvents.Core.Interfaces;

namespace MapsterEvents.API.Controllers
{
    /// <summary>
    /// Etkinlik yönetimi controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventService">Etkinlik servisi</param>
        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Tüm etkinlikleri getirir
        /// </summary>
        /// <returns>Etkinlik listesi</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventListItemDto>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        /// <summary>
        /// Etkinlik detayını getirir
        /// </summary>
        /// <param name="id">Etkinlik ID'si</param>
        /// <returns>Etkinlik detayı</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDetailDto>> GetEventById(int id)
        {
            var eventDetail = await _eventService.GetEventDetailAsync(id);
            
            if (eventDetail == null)
            {
                return NotFound("Etkinlik bulunamadı");
            }

            return Ok(eventDetail);
        }

        /// <summary>
        /// Yaklaşan etkinlikleri getirir
        /// </summary>
        /// <returns>Yaklaşan etkinlik listesi</returns>
        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventListItemDto>>> GetUpcomingEvents()
        {
            var events = await _eventService.GetUpcomingEventsAsync();
            return Ok(events);
        }

        /// <summary>
        /// Geçmiş etkinlikleri getirir
        /// </summary>
        /// <returns>Geçmiş etkinlik listesi</returns>
        [HttpGet("past")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventListItemDto>>> GetPastEvents()
        {
            var events = await _eventService.GetPastEventsAsync();
            return Ok(events);
        }

        /// <summary>
        /// Kategoriye göre etkinlikleri getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventListItemDto>>> GetEventsByCategory(int categoryId)
        {
            var events = await _eventService.GetEventsByCategoryAsync(categoryId);
            return Ok(events);
        }

        /// <summary>
        /// Kullanıcının organize ettiği etkinlikleri getirir
        /// </summary>
        /// <returns>Etkinlik listesi</returns>
        [HttpGet("my-organized")]
        public async Task<ActionResult<IEnumerable<EventListItemDto>>> GetMyOrganizedEvents()
        {
            var userId = GetCurrentUserId();
            var events = await _eventService.GetEventsByOrganizerAsync(userId);
            return Ok(events);
        }

        /// <summary>
        /// Kullanıcının kayıtlı olduğu etkinlikleri getirir
        /// </summary>
        /// <returns>Etkinlik listesi</returns>
        [HttpGet("my-registrations")]
        public async Task<ActionResult<IEnumerable<EventListItemDto>>> GetMyRegisteredEvents()
        {
            var userId = GetCurrentUserId();
            var events = await _eventService.GetUserRegisteredEventsAsync(userId);
            return Ok(events);
        }

        /// <summary>
        /// Yeni etkinlik oluşturur
        /// </summary>
        /// <param name="eventCreateDto">Etkinlik oluşturma DTO'su</param>
        /// <returns>Oluşturulan etkinlik</returns>
        [HttpPost]
        public async Task<ActionResult<EventDetailDto>> CreateEvent([FromBody] EventCreateDto eventCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();
                var createdEvent = await _eventService.CreateEventAsync(eventCreateDto, userId);
                
                return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Etkinlik günceller
        /// </summary>
        /// <param name="id">Etkinlik ID'si</param>
        /// <param name="eventUpdateDto">Etkinlik güncelleme DTO'su</param>
        /// <returns>Güncellenen etkinlik</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<EventDetailDto>> UpdateEvent(int id, [FromBody] EventUpdateDto eventUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();
                var updatedEvent = await _eventService.UpdateEventAsync(id, eventUpdateDto, userId);
                
                if (updatedEvent == null)
                {
                    return NotFound("Etkinlik bulunamadı");
                }

                return Ok(updatedEvent);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Etkinlik siler
        /// </summary>
        /// <param name="id">Etkinlik ID'si</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _eventService.DeleteEventAsync(id, userId);
                
                if (!result)
                {
                    return NotFound("Etkinlik bulunamadı");
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Etkinliğe kayıt olur
        /// </summary>
        /// <param name="id">Etkinlik ID'si</param>
        /// <returns>Kayıt sonucu</returns>
        [HttpPost("{id}/register")]
        public async Task<ActionResult> RegisterToEvent(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _eventService.RegisterToEventAsync(id, userId);
                
                if (result)
                {
                    return Ok("Etkinliğe başarıyla kayıt oldunuz");
                }
                
                return BadRequest("Kayıt işlemi başarısız");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Etkinlik kaydını iptal eder
        /// </summary>
        /// <param name="id">Etkinlik ID'si</param>
        /// <returns>İptal sonucu</returns>
        [HttpDelete("{id}/unregister")]
        public async Task<ActionResult> UnregisterFromEvent(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _eventService.UnregisterFromEventAsync(id, userId);
                
                if (result)
                {
                    return Ok("Etkinlik kaydınız başarıyla iptal edildi");
                }
                
                return BadRequest("Kayıt iptal işlemi başarısız");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Kullanıcının etkinliğe kayıtlı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="id">Etkinlik ID'si</param>
        /// <returns>Kayıt durumu</returns>
        [HttpGet("{id}/registration-status")]
        public async Task<ActionResult<bool>> GetRegistrationStatus(int id)
        {
            var userId = GetCurrentUserId();
            var isRegistered = await _eventService.IsUserRegisteredAsync(id, userId);
            
            return Ok(isRegistered);
        }

        /// <summary>
        /// Mevcut kullanıcının ID'sini getirir
        /// </summary>
        /// <returns>Kullanıcı ID'si</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            
            throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanamadı");
        }
    }
}