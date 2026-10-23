// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.IO;
using System.Text;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    internal sealed class FileHelper : IFileHelper
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
            try
            {
                File.WriteAllText(tempPath, contents, encoding);
                File.Move(tempPath, path, true);
            }
            catch
            {
                // e.g. read-only target: the original file stays untouched, the temporary copy must not stay behind
                try
                {
                    File.Delete(tempPath);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }

                throw;
            }
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