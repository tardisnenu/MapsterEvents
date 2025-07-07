using System.ComponentModel.DataAnnotations;

namespace MapsterEvents.Core.DTOs.Attributes
{
    /// <summary>
    /// Gelecek tarih doğrulama attribute'u
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        private readonly int _minimumMinutesFromNow;
        private readonly DateComparisonType _comparisonType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minimumMinutesFromNow">Şu andan itibaren minimum dakika (varsayılan: 5)</param>
        /// <param name="comparisonType">Karşılaştırma tipi (varsayılan: DateTime)</param>
        public FutureDateAttribute(int minimumMinutesFromNow = 5, DateComparisonType comparisonType = DateComparisonType.DateTime)
        {
            _minimumMinutesFromNow = minimumMinutesFromNow;
            _comparisonType = comparisonType;
            
            ErrorMessage = comparisonType == DateComparisonType.DateOnly 
                ? "Etkinlik tarihi bugün veya sonraki bir tarih olmalıdır"
                : $"Etkinlik tarihi en az {minimumMinutesFromNow} dakika sonrası olmalıdır";
        }

        /// <summary>
        /// Doğrulama işlemi
        /// </summary>
        /// <param name="value">Doğrulanacak değer</param>
        /// <returns>Geçerli mi</returns>
        public override bool IsValid(object? value)
        {
            if (value is not DateTime dateTime)
                return false;

            var now = DateTime.UtcNow;

            return _comparisonType switch
            {
                DateComparisonType.DateOnly => dateTime.Date >= now.Date,
                DateComparisonType.DateTimeExact => dateTime >= now,
                DateComparisonType.DateTime => dateTime >= now.AddMinutes(_minimumMinutesFromNow),
                _ => dateTime > now
            };
        }

        /// <summary>
        /// Hata mesajını formatlar
        /// </summary>
        /// <param name="name">Alan adı</param>
        /// <returns>Formatlanmış hata mesajı</returns>
        public override string FormatErrorMessage(string name)
        {
            return _comparisonType switch
            {
                DateComparisonType.DateOnly => $"{name} bugün veya sonraki bir tarih olmalıdır",
                DateComparisonType.DateTimeExact => $"{name} şu an veya sonrası olmalıdır",
                DateComparisonType.DateTime => $"{name} en az {_minimumMinutesFromNow} dakika sonrası olmalıdır",
                _ => $"{name} gelecekte bir tarih olmalıdır"
            };
        }
    }

    /// <summary>
    /// Tarih karşılaştırma tipi
    /// </summary>
    public enum DateComparisonType
    {
        /// <summary>
        /// Sadece tarih karşılaştırması (saat göz ardı edilir)
        /// </summary>
        DateOnly,
        
        /// <summary>
        /// Tam tarih-saat karşılaştırması (şu an veya sonrası)
        /// </summary>
        DateTimeExact,
        
        /// <summary>
        /// Minimum buffer süreli tarih-saat karşılaştırması
        /// </summary>
        DateTime,
        
        /// <summary>
        /// Strict gelecek tarih (şu anı dahil etmez)
        /// </summary>
        StrictFuture
    }
}