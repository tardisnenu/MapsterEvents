using Microsoft.EntityFrameworkCore;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Repository.Data;

namespace MapsterEvents.Repository.Repositories
{
    /// <summary>
    /// Etkinlik repository implementasyonu
    /// </summary>
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Veritabanı context</param>
        public EventRepository(MapsterEventsDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Yaklaşan etkinlikleri getirir
        /// </summary>
        /// <returns>Yaklaşan etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            return await GetEventsWithBasicIncludes()
                .Where(e => e.Date > DateTime.UtcNow)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Geçmiş etkinlikleri getirir
        /// </summary>
        /// <returns>Geçmiş etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetPastEventsAsync()
        {
            return await GetEventsWithBasicIncludes()
                .Where(e => e.Date <= DateTime.UtcNow)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Kategoriye göre etkinlikleri getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(int categoryId)
        {
            return await GetEventsWithBasicIncludes()
                .Where(e => e.CategoryId == categoryId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Organizatöre göre etkinlikleri getirir
        /// </summary>
        /// <param name="organizerId">Organizatör ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(int organizerId)
        {
            return await GetEventsWithBasicIncludes()
                .Where(e => e.OrganizerId == organizerId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Etkinlikleri detaylarıyla birlikte getirir
        /// </summary>
        /// <returns>Detaylı etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetEventsWithDetailsAsync()
        {
            return await GetEventsWithFullIncludes()
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Etkinlik detayını getirir
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <returns>Detaylı etkinlik</returns>
        public async Task<Event?> GetEventWithDetailsAsync(int eventId)
        {
            return await GetEventsWithFullIncludes()
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        /// <summary>
        /// Kullanıcının kayıtlı olduğu etkinlikleri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetUserRegisteredEventsAsync(int userId)
        {
            return await GetEventsWithBasicIncludes()
                .Where(e => e.Registrations.Any(r => r.UserId == userId))
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Etkinliğe kaydolan kullanıcı sayısını getirir
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <returns>Katılımcı sayısı</returns>
        public async Task<int> GetEventParticipantCountAsync(int eventId)
        {
            return await _context.Registrations
                .Where(r => r.EventId == eventId)
                .CountAsync();
        }

        /// <summary>
        /// Kullanıcının etkinliğe kayıtlı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="eventId">Etkinlik ID'si</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kayıtlı mı</returns>
        public async Task<bool> IsUserRegisteredAsync(int eventId, int userId)
        {
            return await _context.Registrations
                .AnyAsync(r => r.EventId == eventId && r.UserId == userId);
        }

        /// <summary>
        /// Temel include'lar ile etkinlikleri getirir (DRY Helper)
        /// </summary>
        /// <returns>Temel include'lar yapılmış IQueryable</returns>
        private IQueryable<Event> GetEventsWithBasicIncludes()
        {
            return _dbSet
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Registrations);
        }

        /// <summary>
        /// Tam detay include'lar ile etkinlikleri getirir (DRY Helper)
        /// </summary>
        /// <returns>Tam detay include'lar yapılmış IQueryable</returns>
        private IQueryable<Event> GetEventsWithFullIncludes()
        {
            return _dbSet
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Registrations)
                    .ThenInclude(r => r.User);
        }
    }
}