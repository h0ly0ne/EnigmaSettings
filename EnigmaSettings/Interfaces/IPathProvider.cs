// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
 
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IPathProvider
    {
        string Combine(params string[] paths);

        string GetDirectoryName(string fileName);

        string GetFileName(string fileName);
    }
}
