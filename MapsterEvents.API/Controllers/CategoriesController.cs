using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MapsterEvents.Core.DTOs;
using MapsterEvents.Core.Interfaces;

namespace MapsterEvents.API.Controllers
{
    /// <summary>
    /// Kategori yönetimi controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryService">Kategori servisi</param>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Tüm kategorileri getirir
        /// </summary>
        /// <returns>Kategori listesi</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Kategori detayını getirir
        /// </summary>
        /// <param name="id">Kategori ID'si</param>
        /// <returns>Kategori detayı</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            
            if (category == null)
            {
                return NotFound("Kategori bulunamadı");
            }

            return Ok(category);
        }

        /// <summary>
        /// Kategorileri etkinlik sayısıyla birlikte getirir
        /// </summary>
        /// <returns>Kategori listesi</returns>
        [HttpGet("with-event-count")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesWithEventCount()
        {
            var categories = await _categoryService.GetCategoriesWithEventCountAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Yeni kategori oluşturur
        /// </summary>
        /// <param name="categoryCreateDto">Kategori oluşturma DTO'su</param>
        /// <returns>Oluşturulan kategori</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryCreateDto categoryCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(categoryCreateDto);
                
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Kategori günceller
        /// </summary>
        /// <param name="id">Kategori ID'si</param>
        /// <param name="categoryUpdateDto">Kategori güncelleme DTO'su</param>
        /// <returns>Güncellenen kategori</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryUpdateDto);
                
                if (updatedCategory == null)
                {
                    return NotFound("Kategori bulunamadı");
                }

                return Ok(updatedCategory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Kategori siler
        /// </summary>
        /// <param name="id">Kategori ID'si</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                
                if (!result)
                {
                    return NotFound("Kategori bulunamadı");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Kategori adı kullanımda mı kontrol eder
        /// </summary>
        /// <param name="name">Kategori adı</param>
        /// <param name="excludeId">Hariç tutulacak ID</param>
        /// <returns>Kullanım durumu</returns>
        [HttpGet("check-name")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckCategoryName([FromQuery] string name, [FromQuery] int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Kategori adı boş olamaz");
            }

            var isInUse = await _categoryService.IsCategoryNameInUseAsync(name, excludeId);
            return Ok(isInUse);
        }
    }
}