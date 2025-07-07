namespace MapsterEvents.Core.Interfaces
{
    /// <summary>
    /// Kayıt edilebilir entity interface'i (Registration özel zaman damgası için)
    /// </summary>
    public interface IRegistrableEntity
    {
        /// <summary>
        /// Kayıt tarihi
        /// </summary>
        DateTime RegistrationDate { get; set; }
    }
}