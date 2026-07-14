// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

namespace Krkadoni.EnigmaSettings.Interfaces
{
    /// <summary>
    ///     A reference inside a user bouquet to an "alternatives" file
    ///     (alternatives.&lt;name&gt;.tv) holding fallback services for a service.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public interface IBouquetItemAlternative : IBouquetItem
    {
        /// <summary>
        ///     Bouquet holding the alternative services.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IFileBouquet Bouquet { get; set; }

        /// <summary>
        ///     Filename of the alternatives file as referenced in the bouquet line.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string FileName { get; }
    }
}