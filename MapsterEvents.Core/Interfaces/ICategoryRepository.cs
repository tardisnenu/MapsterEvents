using MapsterEvents.Core.Entities;

namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Kategori repository interface
    /// </summary>
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /// <summary>
        /// Kategorileri etkinlikleriyle birlikte getirir (eager loading)
        /// </summary>
        /// <returns>Kategori listesi (Events koleksiyonu dahil)</returns>
        Task<IEnumerable<Category>> GetAllWithEventsAsync();
        
        /// <summary>
        /// Kategoriyi etkinlikleriyle birlikte getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Kategori</returns>
        Task<Category?> GetCategoryWithEventsAsync(int categoryId);
        
        /// <summary>
        /// Kategoride etkinlik var mı kontrol eder
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik var mı</returns>
        Task<bool> HasEventsAsync(int categoryId);
        
        /// <summary>
        /// Kategoriye ait etkinlik sayısını getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik sayısı</returns>
        Task<int> GetEventCountAsync(int categoryId);
        
        /// <summary>
        /// Kategori adına göre kategori getirir
        /// </summary>
        /// <param name="name">Kategori adı</param>
        /// <returns>Kategori</returns>
        Task<Category?> GetCategoryByNameAsync(string name);
        
        /// <summary>
        /// Kategori adı kullanımda mı kontrol eder
        /// </summary>
        /// <param name="name">Kategori adı</param>
        /// <param name="excludeId">Hariç tutulacak ID (güncelleme için)</param>
        /// <returns>Kullanımda mı</returns>
        Task<bool> IsCategoryNameInUseAsync(string name, int? excludeId = null);
    }
}