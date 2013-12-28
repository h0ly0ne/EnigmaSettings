// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IBouquetItemBouquetsBouquet : IBouquetItem
    {
        /// <summary>
        ///     Reference to matcing bouquet instance
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IBouquetsBouquet Bouquet { get; set; }

        /// <summary>
        ///     Negative number in hex format as found in bouquets file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match locked bouquets</remarks>
        string BouquetOrderNumber { get; }

        /// <summary>
        ///     Negative number as integer as found in bouquets file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match locked bouquets</remarks>
        int BouquetOrderNumberInt { get; set; }
    }
}