using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MapsterEvents.Core.Entities;
using MapsterEvents.Core.Interfaces;
using MapsterEvents.Repository.Data;

namespace MapsterEvents.Repository.Repositories
{
    /// <summary>
    /// Generic repository implementasyonu
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly MapsterEventsDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Veritabanı context</param>
        public GenericRepository(MapsterEventsDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Tüm kayıtları getirir
        /// </summary>
        /// <returns>Kayıt listesi</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// ID'ye göre kayıt getirir
        /// </summary>
        /// <param name="id">Kayıt ID'si</param>
        /// <returns>Kayıt</returns>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Şarta göre kayıt getirir
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Kayıt</returns>
        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Şarta göre kayıt listesi getirir
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Kayıt listesi</returns>
        public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Yeni kayıt ekler (Unit of Work pattern - SaveChanges Service layer'da çağırılır)
        /// </summary>
        /// <param name="entity">Eklenecek kayıt</param>
        /// <returns>Eklenen kayıt</returns>
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Kayıt günceller (Unit of Work pattern - SaveChanges Service layer'da çağırılır)
        /// </summary>
        /// <param name="entity">Güncellenecek kayıt</param>
        /// <returns>Güncellenen kayıt</returns>
        public T UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        /// <summary>
        /// Kayıt siler (Unit of Work pattern - SaveChanges Service layer'da çağırılır)
        /// </summary>
        /// <param name="entity">Silinecek kayıt</param>
        /// <returns>Task</returns>
        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// ID'ye göre kayıt siler (Unit of Work pattern - SaveChanges Service layer'da çağırılır)
        /// </summary>
        /// <param name="id">Silinecek kayıt ID'si</param>
        /// <returns>Task</returns>
        public async Task DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        /// <summary>
        /// Kayıt var mı kontrol eder
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Var mı</returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Kayıt sayısını getirir
        /// </summary>
        /// <returns>Kayıt sayısı</returns>
        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        /// <summary>
        /// Şarta göre kayıt sayısını getirir
        /// </summary>
        /// <param name="predicate">Şart</param>
        /// <returns>Kayıt sayısı</returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}