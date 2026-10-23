// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class TransponderDVBT : Transponder, ITransponderDVBT
    {
        /// <summary>
        ///     Initializes new terrestrial transponder from data found in services file
        /// </summary>
        /// <param name="transponderData">0xNamespace:0xTSID:0xNID</param>
        /// <param name="transponderFrequency">
        ///     s
        ///     freq(khz):bandwidth:fecHigh:fec_low:modulation:transmission:guard:hierarchy:inversion:flags
        /// </param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">
        ///     Throws argument null exception if transponderData or transponderFrequency are
        ///     null/empty
        /// </exception>
        /// <exception cref="ArgumentException">Throws argument exception if frequency does not start with 't'</exception>
        public TransponderDVBT(string transponderData, string transponderFrequency)
        {
            if (string.IsNullOrEmpty(transponderData))
                throw new ArgumentNullException(nameof(transponderData));

            if (string.IsNullOrEmpty(transponderFrequency))
                throw new ArgumentNullException(nameof(transponderFrequency));

            _TransponderType = Enums.TransponderType.DVBT;
            string[] tData = transponderData.Split(':');
            string[] tFreq = transponderFrequency.Split(':');

            NameSpc = tData[0];
            TSID = tData[1];
            NID = tData[2];

            if (transponderFrequency.Trim().StartsWith('s'))
            {
                // Enigma1 stores only frequency and inversion; everything else is "auto" in Enigma2 coding,
                // so a conversion to Enigma2 does not force 8 MHz / QPSK / 2k / FEC 1/2
                Frequency = tFreq[0].Split(' ')[1];
                Bandwidth = ((int)Enums.DVBTBandwidthType.Auto).ToString(CultureInfo.InvariantCulture);
                FECHigh = ((int)Enums.DVBTFECHighType.Auto).ToString(CultureInfo.InvariantCulture);
                FECLow = ((int)Enums.DVBTFECLowType.Auto).ToString(CultureInfo.InvariantCulture);
                Modulation = ((int)Enums.DVBTModulationType.Auto).ToString(CultureInfo.InvariantCulture);
                Transmission = ((int)Enums.DVBTTransmissionType.Auto).ToString(CultureInfo.InvariantCulture);
                GuardInterval = ((int)Enums.DVBTGuardIntervalType.Auto).ToString(CultureInfo.InvariantCulture);
                Hierarchy = ((int)Enums.DVBTHierarchyType.Auto).ToString(CultureInfo.InvariantCulture);
                Flags = "0";
                System = "0";
                PlpId = "0";

                if (tFreq.Length > 5)
                    Inversion = tFreq[5];
            }
            else
            {
                Frequency = tFreq[0].Split(' ')[1].Trim();
                Bandwidth = tFreq[1];
                FECHigh = tFreq[2];
                FECLow = tFreq[3];
                Modulation = tFreq[4];

                if (tFreq.Length > 5)
                    Transmission = tFreq[5];

                if (tFreq.Length > 6)
                    GuardInterval = tFreq[6];

                if (tFreq.Length > 7)
                    Hierarchy = tFreq[7];

                if (tFreq.Length > 8)
                    Inversion = tFreq[8];

                if (tFreq.Length > 9)
                    Flags = tFreq[9];

                if (tFreq.Length > 10)
                    System = tFreq[10];

                if (tFreq.Length > 11)
                    PlpId = tFreq[11];
            }
        }

        /// <summary>
        ///     0=8MHz, 1=7MHz, 2=6MHz, 3=Auto, 4=5MHz, 5=1.712MHz, 6=10MHz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public string Bandwidth
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Bandwidth));
                OnPropertyChanged(nameof(BandwidthType));
            }
        } = "0";

        /// <summary>
        ///     Code rate High Pass FEC: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public string FECHigh
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(FECHigh));
                OnPropertyChanged(nameof(FECHighType));
            }
        } = "0";

        /// <summary>
        ///     Code rate Low Pass FEC: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public string FECLow
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(FECLow));
                OnPropertyChanged(nameof(FECLowType));
            }
        } = "0";

        /// <summary>
        ///     0=QPSK, 1=QAM16, 2=QAM64, 3=Auto, 4=QAM256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
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
        } = "0";

        /// <summary>
        ///     0=2k, 1=8k, 2=Auto, 3=4k, 4=1k, 5=16k, 6=32k
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public string Transmission
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Transmission));
                OnPropertyChanged(nameof(TransmissionType));
            }
        } = "0";

        /// <summary>
        ///     0=1/32, 1=1/16, 2=1/8, 3=1/4, 4=Auto, 5=1/128, 6=19/128, 7=19/256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public string GuardInterval
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(GuardInterval));
                OnPropertyChanged(nameof(GuardIntervalType));
            }
        } = "0";

        /// <summary>
        ///     0=None, 1=1, 2=2, 3=4, 4=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public string Hierarchy
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Hierarchy));
                OnPropertyChanged(nameof(HierarchyType));
            }
        } = "0";

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
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
        } = "0";

        /// <summary>
        ///     Transponder flags
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Flags
        {
            get;
            set
            {
                value ??= "0";

                if (value == Flags)
                    return;

                field = value;
                OnPropertyChanged(nameof(Flags));
            }
        } = "0";

        /// <summary>
        ///  https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L793
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

        /// <summary>
        ///  https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L793
        /// </summary>
        [DataMember]
        public string PlpId
        {
            get;
            set
            {
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(PlpId));
            }
        }

        /// <summary>
        ///     Enigma2 coding: 0=8MHz, 1=7MHz, 2=6MHz, 3=Auto, 4=5MHz, 5=1.712MHz, 6=10MHz
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTBandwidthType BandwidthType => ParseCode(Bandwidth, Enums.DVBTBandwidthType.Unknown);

        /// <summary>
        ///     Code rate High Pass FEC, Enigma2 coding: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTFECHighType FECHighType => ParseCode(FECHigh, Enums.DVBTFECHighType.Unknown);

        /// <summary>
        ///     Code rate Low Pass FEC, Enigma2 coding: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTFECLowType FECLowType => ParseCode(FECLow, Enums.DVBTFECLowType.Unknown);

        /// <summary>
        ///     Enigma2 coding: 0=QPSK, 1=QAM16, 2=QAM64, 3=Auto, 4=QAM256
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTModulationType ModulationType => ParseCode(Modulation, Enums.DVBTModulationType.Unknown);

        /// <summary>
        ///     Enigma2 coding: 0=2k, 1=8k, 2=Auto, 3=4k, 4=1k, 5=16k, 6=32k
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTTransmissionType TransmissionType => ParseCode(Transmission, Enums.DVBTTransmissionType.Unknown);

        /// <summary>
        ///     Enigma2 coding: 0=1/32, 1=1/16, 2=1/8, 3=1/4, 4=Auto, 5=1/128, 6=19/128, 7=19/256
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTGuardIntervalType GuardIntervalType => ParseCode(GuardInterval, Enums.DVBTGuardIntervalType.Unknown);

        /// <summary>
        ///     Enigma2 coding: 0=None, 1=1, 2=2, 3=4, 4=Auto
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTHierarchyType HierarchyType => ParseCode(Hierarchy, Enums.DVBTHierarchyType.Unknown);

        /// <summary>
        ///     Enigma2 coding: 0=Off, 1=On, 2=Auto
        /// </summary>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersTerrestrial</see>
        [DataMember]
        public Enums.DVBTInversionType InversionType => ParseCode(Inversion, Enums.DVBTInversionType.Unknown);

        /// <summary>
        ///     Maps a numeric lamedb value to the enum member with the same value
        /// </summary>
        private static T ParseCode<T>(string value, T unknown) where T : struct, Enum
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var code))
                return unknown;

            var result = (T)Enum.ToObject(typeof(T), code);
            return Enum.IsDefined(result) ? result : unknown;
        }

        public override string ToString()
        {
            string tType = string.Join("", "\t", "t", " ", Frequency);
            return string.Join("\t", string.Join(":", NameSpc.PadRight(8, '0'), TSID.PadLeft(4, '0'), NID.PadLeft(4, '0')), string.Join(":", tType, Bandwidth, FECHigh, FECLow, Modulation, Transmission, GuardInterval, Hierarchy, Inversion, Flags), "/");
        }
    }
}