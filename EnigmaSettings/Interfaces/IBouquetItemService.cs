// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IBouquetItemService : IBouquetItem
    {
        /// <summary>
        ///     Used for matching with service from settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string ServiceId { get; }

        /// <summary>
        ///     Service from settings file that this line in bouquet represents
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IService Service { get; set; }
    }
}