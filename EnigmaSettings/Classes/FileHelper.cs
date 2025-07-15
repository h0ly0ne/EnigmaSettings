using System.IO;
using System.Text;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    internal class FileHelper : IFileHelper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void Delete(string path)
        {
             File.Delete(path);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
           File.WriteAllText(path,contents,encoding);
        }

        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateFolder(string path)
        {
           Directory.CreateDirectory(path);
        }
    }
}
