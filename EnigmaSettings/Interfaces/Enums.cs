// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{

    /// <summary>
    /// Static class for all enumeration types
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Type of item in the bouquet (IE. service, marker, stream, another bouquet)
        /// </summary>
        public enum BouquetItemType
        {
            Unknown = -2,
            FileBouquet = 0,
            Service = 1,
            Marker = 2,
            Stream = 3,
            BouquetsBouquet = 4
        }

        /// <summary>
        /// Type of bouquet. Enigma 1 has additional bouquets file with additional bouquets (E1BouquetsBouquet) and services.
        /// </summary>
        public enum BouquetType
        {
            Unknown = -2,
            E1Bouquets = 1,
            E1BouquetsBouquet = 2,
            UserBouquetTv = 3,
            UserBouquetRadio = 4
        }

        /// <summary>
        ///     Known types of C flag in service. If not recognized value is Unknown.
        ///     -3 None
        ///     -2 Unknown
        ///     -1 Empty
        ///     0 Video-PID
        ///     1 Audio-PID
        ///     2 Teletext
        ///     3 PCR
        ///     9 Subtitle
        /// </summary>
        /// <remarks></remarks>
        /// <see href="http://radiovibrations.com/dreambox/services.htm">C flag types</see>
        public enum CFlagType
        {
            None = -3,
            Unknown = -2,
            Empty = -1,
            Video = 0,
            Audio = 1,
            Teletext = 2,
            Pcr = 3,
            Ac3Audio = 4,
            VideoType = 5,
            AudioChannel = 6,
            Ac3Delay = 7,
            PcmDelay = 8,
            Subtitle = 9
        }
        
        public enum DVBCFECInnerType
        {
            Unknown = -2,
            None = 0,
            Auto = 1,
            F12 = 2,
            F23 = 3,
            F34 = 4,
            F56 = 5,
            F78 = 6,
            F89 = 7
        }

        public enum DVBCInversionType
        {
            Unknown = -2,
            Auto = 0,
            On = 1,
            Off = 2
        }

        public enum DVBCModulationType
        {
            Unknown = -2,
            Auto = 0,
            Qam16 = 1,
            Qam32 = 2,
            Qam64 = 3,
            Qam128 = 4,
            Qam256 = 5
        }

        public enum DVBSFECType
        {
            Unknown = -2,
            None = 0,
            Auto = 1,
            F12 = 2,
            F23 = 3,
            F34 = 4,
            F56 = 5,
            F78 = 6,
            F35 = 7,
            F45 = 8,
            F89 = 9,
            F910 = 10
        }

        public enum DVBSInversionType
        {
            Unknown = -2,
            Auto = 0,
            On = 1,
            Off = 2
        }

        public enum DVBSModulationType
        {
            Unknown = -2,
            Auto = 0,
            QPSK = 1,
            Qam16 = 2,
            PSK8 = 3
        }

        public enum DVBSPilotType
        {
            Unknown = -2,
            Auto = 0,
            On = 1,
            Off = 2
        }

        public enum DVBSPolarizationType
        {
            Unknown = -2,
            Horizontal = 0,
            Vertical = 1,
            CircularLeft = 2,
            CircularRight = 3
        }

        public enum DVBSRollOffType
        {
            Unknown = -2,
            X35 = 0,
            X25 = 1,
            X20 = 3
        }

        public enum DVBSSystemType
        {
            Unknown = -2,
            DVBS = 0,
            DVBS2 = 1
        }

        public enum DVBTBandwidthType
        {
            Unknown = -2,
            Auto = 0,
            B8Mhz = 1,
            B7Mhz = 2,
            B6Mhz = 3
        }

        public enum DVBTFECHighType
        {
            Unknown = -2,
            Auto = 0,
            F12 = 1,
            F23 = 2,
            F34 = 3,
            F56 = 4,
            F78 = 5
        }

        public enum DVBTFECLowType
        {
            Unknown = -2,
            Auto = 0,
            F12 = 1,
            F23 = 2,
            F34 = 3,
            F56 = 4,
            F78 = 5
        }

        public enum DVBTGuardIntervalType
        {
            Unknown = -2,
            Auto = 0,
            G132 = 1,
            G116 = 2,
            G18 = 3,
            G14 = 4
        }

        public enum DVBTHierarchyType
        {
            Unknown = -2,
            Auto = 0,
            None = 1,
            H1 = 2,
            H2 = 3,
            H4 = 4
        }

        public enum DVBTInversionType
        {
            Unknown = -2,
            Auto = 0,
            On = 1,
            Off = 2
        }

        public enum DVBTModulationType
        {
            Unknown = -2,
            Auto = 1,
            QPSK = 1,
            Qam16 = 2,
            Qam64 = 3
        }

        public enum DVBTTransmissionType
        {
            Unknown = -2,
            Auto = 0,
            T2K = 1,
            T8K = 3
        }

        /// <summary>
        ///     NoSdt: the servicename will not be automatically updated by the Satellite Data Stream , used to prevent change any
        ///     parameter of a single service
        ///     Dontshow: Hidden service
        ///     NoDVB: ServicePIDs will be retrieved by the Cache, instead of the current Stream
        ///     HoldName: to prevent change service_name
        ///     NewFound: Service found in previous scan
        /// </summary>
        /// <remarks></remarks>
        /// <see href="http://radiovibrations.com/dreambox/services.htm">F flag types</see>
        public enum FFlagType
        {
            None = -3,
            Unknown = -2,
            Empty = -1,
            NoSdt = 1,
            Dontshow = 2,
            NoDVB = 4,
            HoldName = 8,
            NewFound = 64,
            NoSdtDontshow = 3,
            NoSdtNoDVB = 5,
            DontshowNoDVB = 6,
            NoSdtDontshowNoDVB = 7,
            NoSdtHoldName = 9,
            DontshowHoldName = 10,
            NoSdtDontshowHoldName = 11,
            NoDVBHoldName = 12,
            NoSdtNoDVBHoldName = 13,
            DontshowNoDVBHoldName = 14,
            NoSdtDontshowNoDVBHoldName = 15,
            NoSdtNewFound = 65,
            DontshowNewFound = 66,
            NoSdtDontshowNewFound = 67,
            NoDVBNewFound = 68,
            NoSdtNoDVBNewFound = 69,
            DontshowNoDVBNewFound = 70,
            NoSdtDontshowNoDVBNewFound = 71,
            HoldNameNewFound = 72,
            NoSdtHoldNameNewFound = 73,
            DontshowHoldNameNewFound = 74,
            NoSdtDontshowHoldNameNewFound = 75,
            NoDVBHoldNameNewFound = 76,
            NoSdtNoDVBHoldNameNewFound = 77,
            DontshowNoDVBHoldNameNewFound = 78,
            NoSdtDontshowNoDVBHoldNameNewFound = 79
        }

        /// <summary>
        ///     First number in SERVICE line inside one of the bouquets files
        /// </summary>
        /// <remarks></remarks>
        public enum FavoritesType
        {
            Unknown = -2,
            InvalidId = -1,
            StructureId = 0,
            DVBService = 1,
            File = 2,
            Stream = 4097
        }

        /// <summary>
        ///     Type of service flag
        /// </summary>
        /// <remarks></remarks>
        public enum FlagType
        {
            Unknown = -1,
            F = 1,
            P = 2,
            C = 3
        }

        /// <summary>
        ///     Second number in SERVICE line inside one of the bouquets file
        /// </summary>
        /// <remarks></remarks>
        public enum LineSpecifier
        {
            Unknown = -2,
            NormalService = 0,
            IsDirectory = 1,
            MustChangeDirectory = 2,
            IsDirectoryMustChangeDirectory = 3,
            MayChangeDirectory = 4,
            IsDirectoryMayChangeDirectory = 5,
            MustChangeDirectoryMayChangeDirectory = 6,
            IsDirectoryMustChangeDirectoryMayChangeDirectory = 7,
            AutomaticallySorted = 8,
            IsDirectoryAutomaticallySorted = 9,
            MustChangeDirectoryAutomaticallySorted = 10,
            IsDirectoryMustChangeDirectoryAutomaticallySorted = 11,
            MayChangeDirectoryAutomaticallySorted = 12,
            IsDirectoryMayChangeDirectoryAutomaticallySorted = 13,
            MustChangeDirectoryMayChangeDirectoryAutomaticallySorted = 14,
            IsDirectoryMustChangeDirectoryMayChangeDirectoryAutomaticallySorted = 15,
            InternalSortKey = 16,
            SortKeyIs1 = 32,
            Marker = 64,
            ServiceNotPlayable = 128
        }

        /// <summary>
        /// Defines if service is parental locked (blacklisted) or whitelisted
        /// </summary>
        public enum ServiceSecurity
        {
            None = 0,
            WhiteListed = 1,
            BlackListed = 2
        }

        /// <summary>
        ///     DVB Service type as in DVB standard, most used are
        ///     1 -TV, 2-radio, 3-data
        /// </summary>
        /// <remarks></remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd693747(v=vs.85).aspx">Service types</see>
        public enum ServiceType
        {
            Unknown = -2,
            Reserved0 = 0,
            Tv = 1,
            Radio = 2,
            Data = 3,
            NvodMpeg2Sd = 4,
            NvodTimeShifted = 5,
            Mosaic = 6,
            Reserved7 = 7,
            Reserved8 = 8,
            Reserved9 = 9,
            AcRadio = 10,
            AcMosaic = 11,
            DataBroadcastService = 12,
            ReservedCi = 13,
            RcsMap = 14,
            RcsFls = 15,
            DVBMhp = 16,
            Mpeg2Hd = 17,
            Reserved18 = 18,
            Reserved19 = 19,
            Reserved20 = 20,
            Reserved21 = 21,
            AcSdtv = 22,
            AcSdNvodTimeShifted = 23,
            AcSdNvodReference = 24,
            AcHdtv = 25,
            AcHdNvodTimeShifted = 26,
            AcHdNvodReference = 27,
            UserDefined134Bskyb = 134,
            UserDefined135Bskyb = 135
        }

        /// <summary>
        /// Enigma settings version. Can be 
        /// Enigma1V1 (obsolete), 
        /// Enigma1 for Enigma1 receivers 
        /// Enigma2Ver3 and Enigma2Ver4 for Enigma2 receivers
        /// </summary>
        public enum SettingsVersion
        {
            Unknown = -2,
            Enigma1V1 = 1,
            Enigma1 = 2,
            Enigma2Ver3 = 4,
            Enigma2Ver4 = 5
        }

        /// <summary>
        /// Defines if transponder is Satellite, Terrestrial or Cable
        /// </summary>
        public enum TransponderType
        {
            Unknown = -2,
            DVBS = 1,
            DVBT = 2,
            DVBC = 3
        }
    }
}