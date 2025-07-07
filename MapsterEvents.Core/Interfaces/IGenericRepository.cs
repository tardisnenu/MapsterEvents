using System.Linq.Expressions;
using MapsterEvents.Core.Entities;

namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Tüm kayıtları getirir
        /// </summary>
        /// <returns>Kayıt listesi</returns>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// ID'ye göre kayıt getirir
        /// </summary>
        /// <param name="id">Kayıt ID'si</param>
        /// <returns>Kayıt</returns>
        Task<T?> GetByIdAsync(int id);
        
        /// <summary>
        /// Şarta göre kayıt getirir
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Kayıt</returns>
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Şarta göre kayıt listesi getirir
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Kayıt listesi</returns>
        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Yeni kayıt ekler
        /// </summary>
        /// <param name="entity">Eklenecek kayıt</param>
        /// <returns>Eklenen kayıt</returns>
        Task<T> AddAsync(T entity);
        
        /// <summary>
        /// Kayıt günceller
        /// </summary>
        /// <param name="entity">Güncellenecek kayıt</param>
        /// <returns>Güncellenen kayıt</returns>
        T UpdateAsync(T entity);
        
        /// <summary>
        /// Kayıt siler
        /// </summary>
        /// <param name="entity">Silinecek kayıt</param>
        /// <returns>Task</returns>
        Task DeleteAsync(T entity);
        
        /// <summary>
        /// ID'ye göre kayıt siler
        /// </summary>
        /// <param name="id">Silinecek kayıt ID'si</param>
        /// <returns>Task</returns>
        Task DeleteByIdAsync(int id);
        
        /// <summary>
        /// Kayıt var mı kontrol eder
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Var mı</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Kayıt sayısını getirir
        /// </summary>
        /// <returns>Kayıt sayısı</returns>
        Task<int> CountAsync();
        
        /// <summary>
        /// Şarta göre kayıt sayısını getirir
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Kayıt sayısı</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}