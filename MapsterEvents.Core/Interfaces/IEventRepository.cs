using MapsterEvents.Core.Entities;

namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Etkinlik repository interface
    /// </summary>
    public interface IEventRepository : IGenericRepository<Event>
    {
        /// <summary>
        /// Yaklaşan etkinlikleri getirir
        /// </summary>
        /// <returns>Yaklaşan etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        
        /// <summary>
        /// Geçmiş etkinlikleri getirir
        /// </summary>
        /// <returns>Geçmiş etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetPastEventsAsync();
        
        /// <summary>
        /// Kategoriye göre etkinlikleri getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(int categoryId);
        
        /// <summary>
        /// Organizatöre göre etkinlikleri getirir
        /// </summary>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetEventsByOrganizerAsync(int organizerId);
        
        /// <summary>
        /// Etkinlikleri detaylarıyla birlikte getirir
        /// </summary>
        /// <returns>Detaylı etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetEventsWithDetailsAsync();
        
        /// <summary>
        /// Etkinlik detayını getirir
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <returns>Detaylı etkinlik</returns>
        Task<Event?> GetEventWithDetailsAsync(int eventId);
        
        /// <summary>
        /// Kullanıcının kayıtlı olduğu etkinlikleri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetUserRegisteredEventsAsync(int userId);
        
        /// <summary>
        /// Etkinliğe kaydolan kullanıcı sayısını getirir
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <returns>Katılımcı sayısı</returns>
        Task<int> GetEventParticipantCountAsync(int eventId);
        
        /// <summary>
        /// Kullanıcının etkinliğe kayıtlı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kayıtlı mı</returns>
        Task<bool> IsUserRegisteredAsync(int eventId, int userId);
    }
}