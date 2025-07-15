// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ITransponderIPTV : ITransponder
    {
        /// <summary>
        ///     Symbol rate
        /// </summary>
        string SymbolRate { get; set; }

        /// <summary>
        ///     Spectral inversion
        /// </summary>
        Enums.IPTVInversionType InversionType { get; }

        /// <summary>
        ///     Spectral inversion
        /// </summary>
        string Inversion { get; set; }

        /// <summary>
        ///     Modulation
        /// </summary>
        Enums.IPTVModulationType ModulationType { get; }

        /// <summary>
        ///     Modulation
        /// </summary>
        string Modulation { get; set; }

        /// <summary>
        ///     FEC
        /// </summary>
        Enums.IPTVFECType FECType { get; }

        /// <summary>
        ///     FEC
        /// </summary>
        string FEC { get; set; }

        /// <summary>
        ///     Flag value
        /// </summary>
        string Flags { get; set; }

        /// <summary>
        ///     DVB system
        /// </summary>
        string System { get; set; }
    }
}