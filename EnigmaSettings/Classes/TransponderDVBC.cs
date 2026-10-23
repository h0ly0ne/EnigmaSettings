// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Linq;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class TransponderDVBC : Transponder, ITransponderDVBC
    {

        #region "ICloneable"
        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var transponder = (ITransponderDVBC)CloneWithoutSubscribers();
            return transponder;
        }
        #endregion

        /// <summary>
        ///     Initializes new cable transponder from data found in services file
        /// </summary>
        /// <param name="transponderData">0xNamespace:0xTSID:0xNID</param>
        /// <param name="transponderFrequency">c freq(khz):symbolrate(hz):inversion:modulation:fec:flags</param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">
        ///     Throws argument null exception if transponderData or transponderFrequency are
        ///     null/empty
        /// </exception>
        /// <exception cref="ArgumentException">Throws argument exception if frequency does not start with 'c'</exception>
        public TransponderDVBC(string transponderData, string transponderFrequency)
        {
            if (string.IsNullOrEmpty(transponderData))
                throw new ArgumentNullException(nameof(transponderData));

            if (string.IsNullOrEmpty(transponderFrequency))
                throw new ArgumentNullException(nameof(transponderFrequency));

            _TransponderType = Enums.TransponderType.DVBC;
            string[] tData = transponderData.Split(':');
            string[] tFreq = transponderFrequency.Split(':');

            NameSpc = tData[0];
            TSID = tData[1];
            NID = tData[2];

            if (transponderFrequency.TrimStart().StartsWith("c", StringComparison.OrdinalIgnoreCase))
            {
                Frequency = tFreq[0].Split(' ')[1].Trim();
                SymbolRate = tFreq[1];
                if (tFreq.Length > 2)
                    Inversion = tFreq[2];
                if (tFreq.Length > 3)
                    Modulation = tFreq[3];
                if (tFreq.Length > 4)
                    FEC = tFreq[4];
                if (tFreq.Length > 5)
                    Flags = tFreq[5];
                if (tFreq.Length > 6)
                    System = tFreq[6];
            }
            else if (transponderFrequency.Trim().StartsWith('s'))
            {
                // Enigma1 stores cable transponders as satellite lines: s frequency:symbolrate:15:fec:0:inversion,
                // frequency in 10 Hz and symbol rate in 0.1 symbols/s. They are kept in Enigma2 units (kHz,
                // symbols/s) like the Enigma1 writer expects them (it multiplies by 100 / 10 again).
                Frequency = DivideUnit(tFreq[0].Trim().Substring(1).Trim(), "00");
                if (tFreq.Length > 1)
                    SymbolRate = DivideUnit(tFreq[1], "0");
                if (tFreq.Length > 3)
                    FEC = tFreq[3];
                if (tFreq.Length > 5)
                    Inversion = tFreq[5];

                // not stored by Enigma1: auto modulation, default flags and system (DVB-C annex A)
                Modulation = "0";
                Flags = "0";
                System = "0";
            }
        }

        /// <summary>
        ///     Removes the unit suffix ("00" = /100, "0" = /10) from an Enigma1 number; other values are kept as they are
        /// </summary>
        private static string DivideUnit(string value, string suffix)
        {
            value = value?.Trim() ?? string.Empty;
            return value.Length > suffix.Length && value.EndsWith(suffix, StringComparison.Ordinal) && value.All(char.IsDigit)
                ? value.Substring(0, value.Length - suffix.Length)
                : value;
        }

        /// <summary>
        ///     Symbol rate
        /// </summary>
        [DataMember]
        public string SymbolRate
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(SymbolRate));
            }
        }

        /// <summary>
        ///     Spectral inversion
        /// </summary>
        [DataMember]
        public string Inversion
        {
            get;
            set
            {
                value ??= "0";
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Inversion));
                OnPropertyChanged(nameof(InversionType));
            }
        }

        [DataMember]
        public Enums.DVBSCInversionType InversionType
        {
            get
            {
                return Inversion switch
                {
                    "0" => Enums.DVBSCInversionType.Off,
                    "1" => Enums.DVBSCInversionType.On,
                    _ => Enums.DVBSCInversionType.Auto
                };
            }
        }

        /// <summary>
        ///     Modulation
        /// </summary>
        [DataMember]
        public string Modulation
        {
            get;
            set
            {
                value ??= "0";
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Modulation));
                OnPropertyChanged(nameof(ModulationType));
            }
        }

        [DataMember]
        public Enums.DVBCModulationType ModulationType
        {
            get
            {
                return Modulation switch
                {
                    "0" => Enums.DVBCModulationType.Auto,
                    "1" => Enums.DVBCModulationType.QAM16,
                    "2" => Enums.DVBCModulationType.QAM32,
                    "3" => Enums.DVBCModulationType.QAM64,
                    "4" => Enums.DVBCModulationType.QAM128,
                    "5" => Enums.DVBCModulationType.QAM256,
                    _ => Enums.DVBCModulationType.Auto
                };
            }
        }

        /// <summary>
        ///     FEC
        /// </summary>
        [DataMember]
        public string FEC
        {
            get;
            set
            {
                value ??= "0";
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(FEC));
                OnPropertyChanged(nameof(FECType));
            }
        }

        [DataMember]
        public Enums.DVBSCFECType FECType
        {
            get
            {
                return FEC switch
                {
                    "0" => Enums.DVBSCFECType.Auto,
                    "1" => Enums.DVBSCFECType.F12,
                    "2" => Enums.DVBSCFECType.F23,
                    "3" => Enums.DVBSCFECType.F34,
                    "4" => Enums.DVBSCFECType.F56,
                    "5" => Enums.DVBSCFECType.F78,
                    "6" => Enums.DVBSCFECType.F89,
                    "7" => Enums.DVBSCFECType.F35,
                    "8" => Enums.DVBSCFECType.F45,
                    "9" => Enums.DVBSCFECType.F910,
                    "10" => Enums.DVBSCFECType.F67,
                    _ => Enums.DVBSCFECType.None
                };
            }
        }

        /// <summary>
        ///     Flag value
        /// </summary>
        [DataMember]
        public string Flags
        {
            get;
            set
            {
                value ??= "0";
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Flags));
            }
        }

        /// <summary>
        ///     DVB system
        /// </summary>
        [DataMember]
        public string System
        {
            get;
            set
            {
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(System));
            }
        }

        public override string ToString()
        {
            var tType = string.Join("", "\t", "c", " ", Frequency);
            return string.Join("\t", string.Join(":", NameSpc.PadRight(8, '0'), TSID.PadLeft(4, '0'), NID.PadLeft(4, '0')), string.Join(":", tType, SymbolRate, Inversion, Modulation, FEC, Flags, System), "/");
        }
    }
}