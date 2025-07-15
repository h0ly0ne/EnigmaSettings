using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public class DesktopPathProvider : IPathProvider
    {
        public string Combine(params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }

        public string GetDirectoryName(string fileName)
        {
            return System.IO.Path.GetDirectoryName(fileName);
        }

        public string GetFileName(string fileName)
        {
            return System.IO.Path.GetFileName(fileName);
        }
    }
}
