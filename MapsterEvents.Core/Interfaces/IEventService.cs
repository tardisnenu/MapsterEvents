using MapsterEvents.Core.DTOs;

namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Etkinlik servis interface
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Tüm etkinlikleri getirir
        /// </summary>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<EventListItemDto>> GetAllEventsAsync();
        
        /// <summary>
        /// Etkinlik detayını getirir
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <returns>Etkinlik detayı</returns>
        Task<EventDetailDto?> GetEventDetailAsync(int eventId);
        
        /// <summary>
        /// Yaklaşan etkinlikleri getirir
        /// </summary>
        /// <returns>Yaklaşan etkinlik listesi</returns>
        Task<IEnumerable<EventListItemDto>> GetUpcomingEventsAsync();
        
        /// <summary>
        /// Geçmiş etkinlikleri getirir
        /// </summary>
        /// <returns>Geçmiş etkinlik listesi</returns>
        Task<IEnumerable<EventListItemDto>> GetPastEventsAsync();
        
        /// <summary>
        /// Kategoriye göre etkinlikleri getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<EventListItemDto>> GetEventsByCategoryAsync(int categoryId);
        
        /// <summary>
        /// Organizatöre göre etkinlikleri getirir
        /// </summary>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<EventListItemDto>> GetEventsByOrganizerAsync(int organizerId);
        
        /// <summary>
        /// Kullanıcının kayıtlı olduğu etkinlikleri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<EventListItemDto>> GetUserRegisteredEventsAsync(int userId);
        
        /// <summary>
        /// Yeni etkinlik oluşturur
        /// </summary>
        /// <param name="eventCreateDto">Etkinlik oluşturma DTO'su</param>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Oluşturulan etkinlik</returns>
        Task<EventDetailDto> CreateEventAsync(EventCreateDto eventCreateDto, int organizerId);
        
        /// <summary>
        /// Etkinlik günceller
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="eventUpdateDto">Etkinlik güncelleme DTO'su</param>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Güncellenen etkinlik</returns>
        Task<EventDetailDto?> UpdateEventAsync(int eventId, EventUpdateDto eventUpdateDto, int organizerId);
        
        /// <summary>
        /// Etkinlik siler
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Başarılı mı</returns>
        Task<bool> DeleteEventAsync(int eventId, int organizerId);
        
        /// <summary>
        /// Etkinliğe kayıt olur
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Başarılı mı</returns>
        Task<bool> RegisterToEventAsync(int eventId, int userId);
        
        /// <summary>
        /// Etkinlik kaydını iptal eder
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Başarılı mı</returns>
        Task<bool> UnregisterFromEventAsync(int eventId, int userId);
        
        /// <summary>
        /// Kullanıcının etkinliğe kayıtlı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kayıtlı mı</returns>
        Task<bool> IsUserRegisteredAsync(int eventId, int userId);
    }
}