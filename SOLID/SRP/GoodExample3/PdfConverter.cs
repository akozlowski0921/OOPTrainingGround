using System.Text;

namespace SOLID.SRP.Good3
{
    // ✅ Responsibility: PDF conversion
    public class PdfConverter
    {
        public byte[] ConvertHtmlToPdf(string html)
        {
            // In real implementation, use a PDF library
            return Encoding.UTF8.GetBytes(html);
        }
    }
}
