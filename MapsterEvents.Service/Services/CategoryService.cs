using Mapster;
using MapsterEvents.Core.DTOs;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Repository.Data;

namespace MapsterEvents.Service.Services
{
    /// <summary>
    /// Kategori servis implementasyonu
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly MapsterEventsDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryRepository">Kategori repository</param>
        /// <param name="context">Veritabanı context (Unit of Work için)</param>
        public CategoryService(ICategoryRepository categoryRepository, MapsterEventsDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        /// <summary>
        /// Tüm kategorileri getirir
        /// </summary>
        /// <returns>Kategori listesi</returns>
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Adapt<IEnumerable<CategoryDto>>();
        }

        /// <summary>
        /// Kategori detayını getirir
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Kategori detayı</returns>
        public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            return category?.Adapt<CategoryDto>();
        }

        /// <summary>
        /// Kategorileri etkinlik sayısıyla birlikte getirir
        /// </summary>
        /// <returns>Kategori listesi (EventCount hesaplanmış)</returns>
        public async Task<IEnumerable<CategoryDto>> GetCategoriesWithEventCountAsync()
        {
            // Repository'den eager loaded entities'leri al
            var categories = await _categoryRepository.GetAllWithEventsAsync();
            
            // Service layer'da Entity -> DTO mapping (EventCount hesaplama dahil)
            return categories.Adapt<IEnumerable<CategoryDto>>();
        }

        /// <summary>
        /// Yeni kategori oluşturur
        /// </summary>
        /// <param name="categoryCreateDto">Kategori oluşturma DTO'su</param>
        /// <returns>Oluşturulan kategori</returns>
        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryCreateDto)
        {
            // İş kuralı: Kategori adı benzersiz olmalı
            var isNameInUse = await _categoryRepository.IsCategoryNameInUseAsync(categoryCreateDto.Name);
            if (isNameInUse)
            {
                throw new InvalidOperationException("Bu kategori adı zaten kullanılıyor");
            }

            var category = categoryCreateDto.Adapt<Category>();
            var createdCategory = await _categoryRepository.AddAsync(category);
            
            // Unit of Work pattern - Tek transaction içinde save
            await _context.SaveChangesAsync();
            
            return createdCategory.Adapt<CategoryDto>();
        }

        /// <summary>
        /// Kategori günceller
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <param name="categoryUpdateDto">Kategori güncelleme DTO'su</param>
        /// <returns>Güncellenen kategori</returns>
        public async Task<CategoryDto?> UpdateCategoryAsync(int categoryId, CategoryUpdateDto categoryUpdateDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                return null;
            }

            // İş kuralı: Kategori adı benzersiz olmalı (mevcut kategori hariç)
            var isNameInUse = await _categoryRepository.IsCategoryNameInUseAsync(categoryUpdateDto.Name, categoryId);
            if (isNameInUse)
            {
                throw new InvalidOperationException("Bu kategori adı zaten kullanılıyor");
            }

            // Mapster ile DTO'dan entity'ye değerleri kopyala
            categoryUpdateDto.Adapt(existingCategory);

            var updatedCategory = _categoryRepository.UpdateAsync(existingCategory);
            
            // Unit of Work pattern - Tek transaction içinde save
            await _context.SaveChangesAsync();
            
            return updatedCategory.Adapt<CategoryDto>();
        }

        /// <summary>
        /// Kategori siler
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Başarılı mı</returns>
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                return false;
            }

            // İş kuralı: İçinde etkinlik olan kategori silinemez
            var hasEvents = await _categoryRepository.HasEventsAsync(categoryId);
            if (hasEvents)
            {
                throw new InvalidOperationException("İçinde etkinlik olan kategori silinemez");
            }

            await _categoryRepository.DeleteAsync(existingCategory);
            
            // Unit of Work pattern - Tek transaction içinde save
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Kategori adı kullanımda mı kontrol eder
        /// </summary>
        /// <param name="name">Kategori adı</param>
        /// <param name="excludeId">Hariç tutulacak ID</param>
        /// <returns>Kullanımda mı</returns>
        public async Task<bool> IsCategoryNameInUseAsync(string name, int? excludeId = null)
        {
            return await _categoryRepository.IsCategoryNameInUseAsync(name, excludeId);
        }
    }
}