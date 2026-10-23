// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IFileBouquet : IBouquet
    {
        /// <summary>
        ///     Filename of the bouquet without leading directory
        /// </summary>
        /// <value></value>
        /// <returns>Name of the file to be read or to be saved to</returns>
        /// <remarks></remarks>
        string FileName { get; set; }

        /// <summary>
        ///     True when the bouquet is referenced as hidden (LineSpecifier 519) in bouquets.tv/radio.
        /// </summary>
        /// <value></value>
        /// <returns>Current hidden status</returns>
        /// <remarks></remarks>
        bool Hidden { get; set; }

        /// <summary>
        ///     True when the bouquet is only referenced from another bouquet (sub bouquet); it is then written
        ///     as a file but not listed in bouquets.tv / bouquets.radio. False for bouquets listed there.
        /// </summary>
        bool IsSubBouquet { get; set; }

        /// <summary>
        ///     Parental control of the whole bouquet: BlackListed when listed in the Enigma2 blacklist,
        ///     WhiteListed when listed in the whitelist
        /// </summary>
        Enums.ServiceSecurity ServiceSecurity { get; set; }
    }
}