using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CbrConverter.Services
{
    public class FileCollector
    {
        private static readonly string[] SupportedExtensions = { ".cbr", ".cbz", ".rar", ".zip" };

        public IEnumerable<string> CollectSources(string sourcePath, bool recursive)
        {
            if (File.Exists(sourcePath))
                return new[] { sourcePath };

            if (!Directory.Exists(sourcePath))
                return Enumerable.Empty<string>();

            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.GetFiles(sourcePath, "*", option)
                .Where(file => SupportedExtensions.Contains(Path.GetExtension(file).ToLower()));
        }
    }
}
