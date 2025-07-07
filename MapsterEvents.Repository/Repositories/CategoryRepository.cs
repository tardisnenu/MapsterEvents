using Microsoft.EntityFrameworkCore;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Repository.Data;

namespace MapsterEvents.Repository.Repositories
{
    /// <summary>
    /// Kategori repository implementasyonu
    /// </summary>
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Veritabanı context</param>
        public CategoryRepository(MapsterEventsDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Kategorileri etkinlikleriyle birlikte getirir (eager loading)
        /// </summary>
        /// <returns>Kategori listesi (Events koleksiyonu dahil)</returns>
        public async Task<IEnumerable<Category>> GetAllWithEventsAsync()
        {
            return await _dbSet
                .Include(c => c.Events)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Kategoriyi etkinlikleriyle birlikte getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Kategori</returns>
        public async Task<Category?> GetCategoryWithEventsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Events)
                    .ThenInclude(e => e.Organizer)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        /// <summary>
        /// Kategoride etkinlik var mı kontrol eder
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik var mı</returns>
        public async Task<bool> HasEventsAsync(int categoryId)
        {
            return await _context.Events
                .AnyAsync(e => e.CategoryId == categoryId);
        }

        /// <summary>
        /// Kategoriye ait etkinlik sayısını getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Etkinlik sayısı</returns>
        public async Task<int> GetEventCountAsync(int categoryId)
        {
            return await _context.Events
                .Where(e => e.CategoryId == categoryId)
                .CountAsync();
        }

        /// <summary>
        /// Kategori adına göre kategori getirir
        /// </summary>
        /// <param name="name">Kategori adı</param>
        /// <returns>Kategori</returns>
        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Kategori adı kullanımda mı kontrol eder
        /// </summary>
        /// <param name="name">Kategori adı</param>
        /// <param name="excludeId">Hariç tutulacak ID (güncelleme için)</param>
        /// <returns>Kullanımda mı</returns>
        public async Task<bool> IsCategoryNameInUseAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}