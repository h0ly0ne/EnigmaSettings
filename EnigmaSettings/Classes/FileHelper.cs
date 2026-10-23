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
            WriteAllText(path, contents, Utf8NoBom);
        }

        /// <summary>
        ///     Writes to a temporary file first and then replaces the target, so an interrupted save
        ///     never leaves a half-written settings file behind
        /// </summary>
        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, contents, encoding);
            File.Move(tempPath, path, true);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

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