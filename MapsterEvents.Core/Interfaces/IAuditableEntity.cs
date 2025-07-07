namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Audit edilebilir entity interface'i
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Son güncellenme tarihi
        /// </summary>
        DateTime? UpdatedAt { get; set; }
    }
}