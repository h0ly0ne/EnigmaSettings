// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IBouquetItemStream : IBouquetItem
    {
        /// <summary>
        ///     Text as seen in settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Description { get; set; }

        /// <summary>
        ///     URL of the stream
        /// </summary>
        /// <value></value>
        /// <returns>URL decoded string</returns>
        /// <remarks>Will automatically encode string</remarks>
        string URL { get; set; }

        /// <summary>
        ///     Value of the current flag without flag type
        /// </summary>
        /// <value></value>
        /// <returns>Hex value</returns>
        /// <remarks></remarks>
        string StreamFlag { get; set; }

        /// <summary>
        ///     StreamFlag value as integer
        /// </summary>
        /// <value></value>
        /// <returns>0 if flag is not valid hex</returns>
        /// <remarks></remarks>
        int StreamFlagInt { get; set; }

        /// <summary>
        ///     Extra flag as 5th item in service line
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>In hex format</remarks>
        string ExtraFlag1 { get; set; }

        /// <summary>
        ///     Extra flag as 5th item in service line
        /// </summary>
        /// <value></value>
        /// <returns>Integer value of flag</returns>
        /// <remarks></remarks>
        int ExtraFlag1Int { get; set; }

        /// <summary>
        ///     Extra flag as 6th item in service line
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>In hex format</remarks>
        string ExtraFlag2 { get; set; }

        /// <summary>
        ///     Extra flag as 6th item in service line
        /// </summary>
        /// <value></value>
        /// <returns>Integer value of flag</returns>
        /// <remarks></remarks>
        int ExtraFlag2Int { get; set; }

        /// <summary>
        ///     Extra flag as 4th item in service line
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>In hex format</remarks>
        string ServiceID { get; set; }

        /// <summary>
        ///     Extra flag as 4th item in service line
        /// </summary>
        /// <value></value>
        /// <returns>Integer value of flag</returns>
        /// <remarks></remarks>
        int ServiceIDInt { get; set; }
    }
}