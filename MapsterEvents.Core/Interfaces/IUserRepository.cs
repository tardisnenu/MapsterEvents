using MapsterEvents.Core.Entities;

namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Kullanıcı repository interface
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// E-posta adresine göre kullanıcı getirir
        /// </summary>
        /// <param name="email">E-posta adresi</param>
        /// <returns>Kullanıcı</returns>
        Task<User?> GetUserByEmailAsync(string email);
        
        /// <summary>
        /// E-posta adresi kullanımda mı kontrol eder
        /// </summary>
        /// <param name="email">E-posta adresi</param>
        /// <param name="excludeId">Hariç tutulacak ID (güncelleme için)</param>
        /// <returns>Kullanımda mı</returns>
        Task<bool> IsEmailInUseAsync(string email, int? excludeId = null);
        
        /// <summary>
        /// Kullanıcının organize ettiği etkinlikleri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetUserOrganizedEventsAsync(int userId);
        
        /// <summary>
        /// Kullanıcının kayıtlı olduğu etkinlikleri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Etkinlik listesi</returns>
        Task<IEnumerable<Event>> GetUserRegisteredEventsAsync(int userId);
        
        /// <summary>
        /// Kullanıcının etkinlik istatistiklerini getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>İstatistikler</returns>
        Task<(int OrganizedEvents, int RegisteredEvents)> GetUserEventStatsAsync(int userId);
    }
}