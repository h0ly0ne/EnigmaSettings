// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

namespace Krkadoni.EnigmaSettings.Interfaces
{
    /// <summary>
    ///     A reference inside a user bouquet to an "alternatives" file
    ///     (alternatives.&lt;name&gt;.tv) holding fallback services for a service.
    /// </summary>
    public interface IBouquetItemAlternative : IBouquetItem
    {
        /// <summary>Bouquet holding the alternative services.</summary>
        IFileBouquet Bouquet { get; set; }

        /// <summary>Filename of the alternatives file as referenced in the bouquet line.</summary>
        string FileName { get; }
    }
}
