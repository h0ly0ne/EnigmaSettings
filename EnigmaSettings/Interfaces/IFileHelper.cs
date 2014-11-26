// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
 
using System.Text;

namespace Krkadoni.EnigmaSettings.Interfaces
{
   public interface IFileHelper
   {
       bool Exists(string path);

       string[] ReadAllLines(string path);

       string ReadAllText(string path);

       void Delete(string path);

       void WriteAllText(string path, string contents);

       void WriteAllText(string path, string contents, Encoding encoding);

       bool FolderExists(string path);

       void CreateFolder(string path);

    }
}
