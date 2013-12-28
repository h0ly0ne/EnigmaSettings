// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IBouquetsBouquet : IBouquet
    {
        /// <summary>
        ///     Determines if bouquet is locked.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Locked bouquets are stored inside services.locked file</remarks>
        bool Locked { get; set; }
    }
}