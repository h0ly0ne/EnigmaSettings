// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IBouquetItemFileBouquet : IBouquetItem
    {
        /// <summary>
        ///     Reference to matcing bouquet instance
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IFileBouquet Bouquet { get; set; }

        /// <summary>
        ///     Bouquet filename
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match bouquet reference</remarks>
        string FileName { get; }
    }
}