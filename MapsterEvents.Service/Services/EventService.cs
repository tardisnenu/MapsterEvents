using Mapster;
using MapsterEvents.Core.DTOs;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace MapsterEvents.Service.Services
{
    /// <summary>
    /// Etkinlik servis implementasyonu
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly MapsterEventsDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventRepository">Etkinlik repository</param>
        /// <param name="categoryRepository">Kategori repository</param>
        /// <param name="userRepository">Kullanıcı repository</param>
        /// <param name="context">Veritabanı context (Unit of Work için)</param>
        public EventService(IEventRepository eventRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, MapsterEventsDbContext context)
        {
            _eventRepository = eventRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _context = context;
        }

        /// <summary>
        /// Tüm etkinlikleri getirir
        /// </summary>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<EventListItemDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetEventsWithDetailsAsync();
            return events.Adapt<IEnumerable<EventListItemDto>>();
        }

        /// <summary>
        /// Etkinlik detayını getirir
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <returns>Etkinlik detayı</returns>
        public async Task<EventDetailDto?> GetEventDetailAsync(int eventId)
        {
            var eventEntity = await _eventRepository.GetEventWithDetailsAsync(eventId);
            return eventEntity?.Adapt<EventDetailDto>();
        }

        /// <summary>
        /// Yaklaşan etkinlikleri getirir
        /// </summary>
        /// <returns>Yaklaşan etkinlik listesi</returns>
        public async Task<IEnumerable<EventListItemDto>> GetUpcomingEventsAsync()
        {
            var events = await _eventRepository.GetUpcomingEventsAsync();
            return events.Adapt<IEnumerable<EventListItemDto>>();
        }

        /// <summary>
        /// Geçmiş etkinlikleri getirir
        /// </summary>
        /// <returns>Geçmiş etkinlik listesi</returns>
        public async Task<IEnumerable<EventListItemDto>> GetPastEventsAsync()
        {
            var events = await _eventRepository.GetPastEventsAsync();
            return events.Adapt<IEnumerable<EventListItemDto>>();
        }

        /// <summary>
        /// Kategoriye göre etkinlikleri getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<EventListItemDto>> GetEventsByCategoryAsync(int categoryId)
        {
            var events = await _eventRepository.GetEventsByCategoryAsync(categoryId);
            return events.Adapt<IEnumerable<EventListItemDto>>();
        }

        /// <summary>
        /// Organizatöre göre etkinlikleri getirir
        /// </summary>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<EventListItemDto>> GetEventsByOrganizerAsync(int organizerId)
        {
            var events = await _eventRepository.GetEventsByOrganizerAsync(organizerId);
            return events.Adapt<IEnumerable<EventListItemDto>>();
        }

        /// <summary>
        /// Kullanıcının kayıtlı olduğu etkinlikleri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<EventListItemDto>> GetUserRegisteredEventsAsync(int userId)
        {
            var events = await _eventRepository.GetUserRegisteredEventsAsync(userId);
            return events.Adapt<IEnumerable<EventListItemDto>>();
        }

        /// <summary>
        /// Yeni etkinlik oluşturur
        /// </summary>
        /// <param name="eventCreateDto">Etkinlik oluşturma DTO'su</param>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Oluşturulan etkinlik</returns>
        public async Task<EventDetailDto> CreateEventAsync(EventCreateDto eventCreateDto, int organizerId)
        {
            // İş kuralı: Geçmiş tarihli etkinlik oluşturulamaz
            if (eventCreateDto.Date <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Geçmiş tarihli etkinlik oluşturulamaz");
            }

            // Kategori kontrolü
            var category = await _categoryRepository.GetByIdAsync(eventCreateDto.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Geçersiz kategori");
            }

            // Organizatör kontrolü
            var organizer = await _userRepository.GetByIdAsync(organizerId);
            if (organizer == null)
            {
                throw new InvalidOperationException("Geçersiz organizatör");
            }

            var eventEntity = eventCreateDto.Adapt<Event>();
            eventEntity.OrganizerId = organizerId;

            var createdEvent = await _eventRepository.AddAsync(eventEntity);
            
            // Unit of Work pattern - Tek transaction içinde save
            await _context.SaveChangesAsync();
            
            // Performans optimizasyonu: Navigation property'leri manuel yükle
            await _context.Entry(createdEvent)
                .Reference(e => e.Category)
                .LoadAsync();
            await _context.Entry(createdEvent)
                .Reference(e => e.Organizer)
                .LoadAsync();
            await _context.Entry(createdEvent)
                .Collection(e => e.Registrations)
                .LoadAsync();
            
            // Ekstra SELECT sorgusu yapmadan direkt DTO'ya çevir
            return createdEvent.Adapt<EventDetailDto>();
        }

        /// <summary>
        /// Etkinlik günceller
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="eventUpdateDto">Etkinlik güncelleme DTO'su</param>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Güncellenen etkinlik</returns>
        public async Task<EventDetailDto?> UpdateEventAsync(int eventId, EventUpdateDto eventUpdateDto, int organizerId)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null)
            {
                return null;
            }

            // İş kuralı: Sadece organizatör kendi etkinliğini güncelleyebilir
            if (existingEvent.OrganizerId != organizerId)
            {
                throw new UnauthorizedAccessException("Bu etkinliği güncelleme yetkiniz yok");
            }

            // İş kuralı: Geçmiş tarihli etkinlik oluşturulamaz
            if (eventUpdateDto.Date <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Geçmiş tarihli etkinlik oluşturulamaz");
            }

            // Kategori kontrolü
            var category = await _categoryRepository.GetByIdAsync(eventUpdateDto.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Geçersiz kategori");
            }

            // Mapster ile DTO'dan entity'ye değerleri kopyala
            eventUpdateDto.Adapt(existingEvent);

            _eventRepository.UpdateAsync(existingEvent);
            
            // Unit of Work pattern - Tek transaction içinde save
            await _context.SaveChangesAsync();
            
            // Performans optimizasyonu: Navigation property'leri manuel yükle
            await _context.Entry(existingEvent)
                .Reference(e => e.Category)
                .LoadAsync();
            await _context.Entry(existingEvent)
                .Reference(e => e.Organizer)
                .LoadAsync();
            await _context.Entry(existingEvent)
                .Collection(e => e.Registrations)
                .Query()
                .Include(r => r.User)
                .LoadAsync();
            
            // Ekstra SELECT sorgusu yapmadan direkt DTO'ya çevir
            return existingEvent.Adapt<EventDetailDto>();
        }

        /// <summary>
        /// Etkinlik siler
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Başarılı mı</returns>
        public async Task<bool> DeleteEventAsync(int eventId, int organizerId)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null)
            {
                return false;
            }

            // İş kuralı: Sadece organizatör kendi etkinliğini silebilir
            if (existingEvent.OrganizerId != organizerId)
            {
                throw new UnauthorizedAccessException("Bu etkinliği silme yetkiniz yok");
            }

            await _eventRepository.DeleteAsync(existingEvent);
            
            // Unit of Work pattern - Tek transaction içinde save
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Etkinliğe kayıt olur
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Başarılı mı</returns>
        public async Task<bool> RegisterToEventAsync(int eventId, int userId)
        {
            // Etkinlik kontrolü
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                throw new InvalidOperationException("Etkinlik bulunamadı");
            }

            // İş kuralı: Geçmiş etkinliğe kayıt olunamaz
            if (eventEntity.Date <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Geçmiş etkinliğe kayıt olunamaz");
            }

            // Kullanıcı kontrolü
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("Kullanıcı bulunamadı");
            }

            // İş kuralı: Zaten kayıtlı kullanıcı tekrar kayıt olamaz
            var isAlreadyRegistered = await _eventRepository.IsUserRegisteredAsync(eventId, userId);
            if (isAlreadyRegistered)
            {
                throw new InvalidOperationException("Bu etkinliğe zaten kayıtlısınız");
            }

            // Kayıt oluştur - RegistrationDate DbContext tarafından otomatik atanacak
            var registration = new Registration
            {
                EventId = eventId,
                UserId = userId
                // RegistrationDate - IRegistrableEntity ile DbContext otomatik atar
            };

            // Registration için repository yok - Doğrudan context kullan
            await _context.Registrations.AddAsync(registration);
            
            // Unit of Work pattern - Tek transaction içinde save
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Etkinlik kaydını iptal eder
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Başarılı mı</returns>
        public async Task<bool> UnregisterFromEventAsync(int eventId, int userId)
        {
            // Kayıt kontrolü
            var isRegistered = await _eventRepository.IsUserRegisteredAsync(eventId, userId);
            if (!isRegistered)
            {
                throw new InvalidOperationException("Bu etkinliğe kayıtlı değilsiniz");
            }

            // Registration silme işlemi
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
                
            if (registration != null)
            {
                _context.Registrations.Remove(registration);
                
                // Unit of Work pattern - Tek transaction içinde save
                await _context.SaveChangesAsync();
            }
            
            return true;
        }

        /// <summary>
        /// Kullanıcının etkinliğe kayıtlı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kayıtlı mı</returns>
        public async Task<bool> IsUserRegisteredAsync(int eventId, int userId)
        {
            return await _eventRepository.IsUserRegisteredAsync(eventId, userId);
        }
    }
}