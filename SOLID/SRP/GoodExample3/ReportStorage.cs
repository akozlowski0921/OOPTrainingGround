using System.IO;

namespace SOLID.SRP.Good3
{
    // ✅ Responsibility: File storage
    public class ReportStorage
    {
        private readonly string _storageDirectory;

        public ReportStorage(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
        }

        public void Save(string fileName, byte[] content)
        {
            var fullPath = Path.Combine(_storageDirectory, fileName);
            File.WriteAllBytes(fullPath, content);
        }
    }
}
