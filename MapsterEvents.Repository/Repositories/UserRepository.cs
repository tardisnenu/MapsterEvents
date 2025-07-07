using Microsoft.EntityFrameworkCore;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Repository.Data;

namespace MapsterEvents.Repository.Repositories
{
    /// <summary>
    /// Kullanıcı repository implementasyonu
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Veritabanı context</param>
        public UserRepository(MapsterEventsDbContext context) : base(context)
        {
        }

        /// <summary>
        /// E-posta adresine göre kullanıcı getirir
        /// </summary>
        /// <param name="email">E-posta adresi</param>
        /// <returns>Kullanıcı</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        /// <summary>
        /// E-posta adresi kullanımda mı kontrol eder
        /// </summary>
        /// <param name="email">E-posta adresi</param>
        /// <param name="excludeId">Hariç tutulacak ID (güncelleme için)</param>
        /// <returns>Kullanımda mı</returns>
        public async Task<bool> IsEmailInUseAsync(string email, int? excludeId = null)
        {
            var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        /// <summary>
        /// Kullanıcının organize ettiği etkinlikleri getirir
        /// Note: Pragmatik yaklaşım - _context.Events'e doğrudan erişim
        /// Alternatif: IEventRepository inject edip GetEventsByOrganizerAsync kullanmak
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetUserOrganizedEventsAsync(int userId)
        {
            return await _context.Events
                .Where(e => e.OrganizerId == userId)
                .Include(e => e.Category)
                .Include(e => e.Registrations)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Kullanıcının kayıtlı olduğu etkinlikleri getirir
        /// Note: Pragmatik yaklaşım - _context.Events'e doğrudan erişim
        /// Alternatif: IEventRepository inject edip GetUserRegisteredEventsAsync kullanmak
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        public async Task<IEnumerable<Event>> GetUserRegisteredEventsAsync(int userId)
        {
            return await _context.Events
                .Where(e => e.Registrations.Any(r => r.UserId == userId))
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Include(e => e.Registrations)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Kullanıcının etkinlik istatistiklerini getirir (Optimize: Single query)
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>İstatistikler</returns>
        public async Task<(int OrganizedEvents, int RegisteredEvents)> GetUserEventStatsAsync(int userId)
        {
            // Tek sorgu ile her iki sayıyı da al - Performance optimized
            var stats = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new 
                {
                    OrganizedEvents = u.OrganizedEvents.Count(),
                    RegisteredEvents = u.Registrations.Count()
                })
                .FirstOrDefaultAsync();

            return stats != null ? (stats.OrganizedEvents, stats.RegisteredEvents) : (0, 0);
        }
    }
}