// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IBouquetItemMarker : IBouquetItem
    {
        /// <summary>
        ///     Text of marker visible in settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Description { get; set; }

        /// <summary>
        ///     Sequential number of marker in settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        int MarkerNumber { get; set; }
    }
}