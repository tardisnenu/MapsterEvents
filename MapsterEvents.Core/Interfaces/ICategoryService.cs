using MapsterEvents.Core.DTOs;

namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Kategori servis interface
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Tüm kategorileri getirir
        /// </summary>
        /// <returns>Kategori listesi</returns>
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        
        /// <summary>
        /// Kategori detayını getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Kategori detayı</returns>
        Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);
        
        /// <summary>
        /// Kategorileri etkinlik sayısıyla birlikte getirir
        /// </summary>
        /// <returns>Kategori listesi</returns>
        Task<IEnumerable<CategoryDto>> GetCategoriesWithEventCountAsync();
        
        /// <summary>
        /// Yeni kategori oluşturur
        /// </summary>
        /// <param name="categoryCreateDto">Kategori oluşturma DTO'su</param>
        /// <returns>Oluşturulan kategori</returns>
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryCreateDto);
        
        /// <summary>
        /// Kategori günceller
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <param name="categoryUpdateDto">Kategori güncelleme DTO'su</param>
        /// <returns>Güncellenen kategori</returns>
        Task<CategoryDto?> UpdateCategoryAsync(int categoryId, CategoryUpdateDto categoryUpdateDto);
        
        /// <summary>
        /// Kategori siler
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Başarılı mı</returns>
        Task<bool> DeleteCategoryAsync(int categoryId);
        
        /// <summary>
        /// Kategori adı kullanımda mı kontrol eder
        /// </summary>
        /// <param name="name">Kategori adı</param>
        /// <param name="excludeId">Hariç tutulacak ID</param>
        /// <returns>Kullanımda mı</returns>
        Task<bool> IsCategoryNameInUseAsync(string name, int? excludeId = null);
    }
}