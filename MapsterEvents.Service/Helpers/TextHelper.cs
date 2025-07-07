using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace MapsterEvents.Service.Helpers
{
    /// <summary>
    /// Metin işleme yardımcı sınıfı - Güvenli HTML sanitization ile
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// Metni belirtilen uzunlukta kısaltır ve kelime sınırlarına uyar
        /// </summary>
        /// <param name="text">Kısaltılacak metin</param>
        /// <param name="maxLength">Maksimum uzunluk</param>
        /// <returns>Kısaltılmış metin</returns>
        public static string TruncateAtWord(string text, int maxLength = 150)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLength)
                return text;

            // Maksimum uzunluğu aş, ama kelime sınırında kes
            var truncated = text.Substring(0, maxLength);
            var lastSpaceIndex = truncated.LastIndexOf(' ');
            
            if (lastSpaceIndex > 0)
            {
                truncated = truncated.Substring(0, lastSpaceIndex);
            }
            
            return truncated.TrimEnd('.', ',', ';', '!', '?') + "...";
        }

        /// <summary>
        /// HTML etiketlerini güvenli bir şekilde temizler (XSS koruması ile)
        /// HtmlAgilityPack kullanarak proper DOM parsing
        /// </summary>
        /// <param name="html">HTML içeriği</param>
        /// <returns>Güvenli temizlenmiş metin</returns>
        public static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            try
            {
                // HtmlAgilityPack ile güvenli HTML parsing
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                
                // Sadece text node'larını al, HTML etiketlerini atla
                var textContent = doc.DocumentNode.InnerText;
                
                // HTML entities'leri decode et (&amp; -> &, &lt; -> < vb.)
                var decodedText = System.Net.WebUtility.HtmlDecode(textContent);
                
                // Çoklu boşlukları tek boşluğa çevir ve temizle
                var cleanText = Regex.Replace(decodedText, @"\s+", " ");
                
                return cleanText.Trim();
            }
            catch (Exception)
            {
                // Eğer HTML parsing başarısız olursa, fallback olarak boş string dön
                // Güvenlik açısından potansiyel zararsız içeriği riske atmamak için
                return string.Empty;
            }
        }

        /// <summary>
        /// Metni kısaltır ve HTML etiketlerini güvenli bir şekilde temizler
        /// </summary>
        /// <param name="htmlText">HTML içeren metin</param>
        /// <param name="maxLength">Maksimum uzunluk</param>
        /// <returns>Güvenli temizlenmiş ve kısaltılmış metin</returns>
        public static string CreateSummary(string htmlText, int maxLength = 150)
        {
            var cleanText = StripHtml(htmlText);
            return TruncateAtWord(cleanText, maxLength);
        }

        /// <summary>
        /// HTML içeriğini güvenli bir şekilde sanitize eder (whitelist yaklaşımı)
        /// Sadece güvenli HTML etiketlerine izin verir
        /// </summary>
        /// <param name="html">Sanitize edilecek HTML</param>
        /// <returns>Güvenli HTML</returns>
        public static string SanitizeHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Güvenli etiketler listesi (whitelist)
                var allowedTags = new[] { "p", "br", "strong", "b", "em", "i", "u", "ul", "ol", "li" };
                
                // Zararsız olmayan tüm etiketleri kaldır
                var nodesToRemove = doc.DocumentNode
                    .Descendants()
                    .Where(node => node.NodeType == HtmlNodeType.Element && 
                                   !allowedTags.Contains(node.Name.ToLower()))
                    .ToList();

                foreach (var node in nodesToRemove)
                {
                    // Etiketi kaldır ama içeriğini koru
                    node.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(node.InnerText), node);
                }

                // Tüm attribute'ları kaldır (onclick, onload vb. XSS vektörlerini engeller)
                foreach (var node in doc.DocumentNode.Descendants().Where(n => n.NodeType == HtmlNodeType.Element))
                {
                    node.Attributes.RemoveAll();
                }

                return doc.DocumentNode.InnerHtml;
            }
            catch (Exception)
            {
                // Güvenlik açısından hatalı HTML'i riske atmak yerine boş dön
                return string.Empty;
            }
        }
    }
}