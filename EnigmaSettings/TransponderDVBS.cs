// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Globalization;
using Krkadoni.EnigmaSettings.Interfaces;
using Krkadoni.EnigmaSettings.Properties;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public class TransponderDVBS : Transponder, ITransponderDVBS
    {

        #region "IEditable"

        private bool _isEditing;
        private string _mFec;
        private string _mFlags;
        private string _mInversion;
        private string _mModulation;
        private int _mOrbitalPositionInt;
        private string _mPilot;
        private string _mPolarization;
        private string _mRollOff;
        private IXmlSatellite _mSatellite;
        private string _mSymbolRate;
        private string _mSystem;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) return;
            _mFec = _fec;
            _mFlags = _flags;
            _mInversion = _inversion;
            _mModulation = _modulation;
            _mOrbitalPositionInt = _orbitalPositionInt;
            _mPilot = _pilot;
            _mPolarization = _polarization;
            _mRollOff = _rollOff;
            _mSatellite = _satellite;
            _mSymbolRate = _symbolRate;
            _mSystem = _system;
            _isEditing = true;
        }

        public override void EndEdit()
        {
            base.EndEdit();
            _isEditing = false;
        }

        public override void CancelEdit()
        {
            base.CancelEdit();
            if (!_isEditing) return;
            FEC = _mFec;
            Flags = _mFlags;
            Inversion = _mInversion;
            Modulation = _mModulation;
            OrbitalPositionInt = _mOrbitalPositionInt;
            Pilot = _mPilot;
            Polarization = _mPolarization;
            RollOff = _mRollOff;
            Satellite = _mSatellite;
            SymbolRate = _mSymbolRate;
            System = _mSystem;
            _isEditing = false;
        }

        #endregion

        private string _fec = "0";
        private string _flags;
        private string _inversion = "0";
        private string _modulation;
        private int _orbitalPositionInt;
        private string _pilot;
        private string _polarization = "0";
        private string _rollOff;
        private IXmlSatellite _satellite;

        private string _symbolRate = "0";
        private string _system;

        /// <summary>
        ///     Initializes new satelite transponder from data found in services file
        /// </summary>
        /// <param name="transponderData">0xNamespace:0xTSID:0xNID</param>
        /// <param name="transponderFrequency">
        ///     s
        ///     freq(khz):symbolrate(hz):pol:fec:pos:inversion:flags[:system:modulation:rolloff:pilot]
        /// </param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">
        ///     Throws argument null exception if transponderData or transponderFrequency are
        ///     null/empty
        /// </exception>
        /// <exception cref="ArgumentException">Throws argument exception if frequency does not start with 's'</exception>
        public TransponderDVBS(string transponderData, string transponderFrequency)
        {
            if (string.IsNullOrEmpty(transponderData))
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(transponderFrequency))
                throw new ArgumentNullException();
            _TransponderType = Enums.TransponderType.DVBS;
            string[] tData = transponderData.Split(':');
            string[] tFreq = transponderFrequency.Split(':');
            if (!tFreq[0].Split(' ')[0].Trim().ToLower().StartsWith("s"))
            {
                throw new ArgumentException(Resources.Transponder_New_Invalid_transponder_type_);
            }

            NameSpc = tData[0];
            TSID = tData[1];
            NID = tData[2];
            Frequency = tFreq[0].Split(' ')[1].Trim();
            SymbolRate = tFreq[1];
            Polarization = tFreq[2];
            FEC = tFreq[3];
            OrbitalPositionInt = Convert.ToInt32(tFreq[4]);
            Inversion = tFreq[5];
            if (tFreq.Length > 6)
                Flags = tFreq[6];
            if (tFreq.Length > 7)
                System = tFreq[7];
            if (tFreq.Length > 8)
                Modulation = tFreq[8];
            if (tFreq.Length > 9)
                RollOff = tFreq[9];
            if (tFreq.Length > 10)
                Pilot = tFreq[10];
        }

        /// <summary>
        ///     Instance of satellite from satellites.xml transponder belongs to
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IXmlSatellite Satellite
        {
            get { return _satellite; }
            set
            {
                if (value == _satellite) return;
                _satellite = value;
                OnPropertyChanged("Satellite");
            }
        }

        /// <summary>
        ///     Transponder symbol rate
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SymbolRate
        {
            get { return _symbolRate; }
            set
            {
                if (value == null)
                    value = "0";
                if (value != _symbolRate)
                {
                    OnPropertyChanged("SymbolRate");
                }
                _symbolRate = value;
            }
        }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Polarization values</see>
        public string Polarization
        {
            get { return _polarization; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _polarization) return;
                _polarization = value;
                OnPropertyChanged("Polarization");
                OnPropertyChanged("PolarizationType");
            }
        }

        /// <summary>
        ///     0=None , 1=Auto, 2=1/2, 3=2/3, 4=3/4 5=5/6, 6=7/8, 7=3/5, 8=4/5, 9=8/9, 10=9/10
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        public string FEC
        {
            get { return _fec; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _fec) return;
                _fec = value;
                OnPropertyChanged("FEC");
                OnPropertyChanged("FECType");
                OnPropertyChanged("Flags");

            }
        }

        /// <summary>
        ///     Orbital position value as integer
        /// </summary>
        /// <value>Position as integer number with length 3 (ie. 19.2E = 192) </value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int OrbitalPositionInt
        {
            get { return _orbitalPositionInt; }
            set
            {
                if (value == _orbitalPositionInt) return;
                _orbitalPositionInt = value;
                OnPropertyChanged("OrbitalPositionInt");
                OnPropertyChanged("OrbitalPositionHex");
            }
        }

        /// <summary>
        ///     Orbital position of corresponding satellite as Hex value.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string OrbitalPositionHex
        {
            get { return _orbitalPositionInt.ToString("X"); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = "0";
                try
                {
                    OrbitalPositionInt = Int32.Parse(value, NumberStyles.HexNumber);
                }
                catch (Exception)
                {
                    OrbitalPositionInt = 0;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        public string Inversion
        {
            get { return _inversion; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _inversion) return;
                _inversion = value;
                OnPropertyChanged("Inversion");
                OnPropertyChanged("InversionType");
            }
        }

        /// <summary>
        ///     Only in version 4. Field is absent in version 3
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Flags
        {
            get { return _flags; }
            set
            {
                if (value == Flags) return;
                _flags = value;
                OnPropertyChanged("Flags");
            }
        }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">System types</see>
        public string System
        {
            get { return _system; }
            set
            {
                if (value == _system) return;
                _system = value;
                OnPropertyChanged("System");
                OnPropertyChanged("SystemType");
            }
        }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=8PSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        public string Modulation
        {
            get { return _modulation; }
            set
            {
                if (value == _modulation) return;
                _modulation = value;
                OnPropertyChanged("Modulation");
                OnPropertyChanged("ModulationType");
            }
        }

        /// <summary>
        ///     Only used in DVB-S2. 0=0.35, 1=0.25, 3=0.20
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string RollOff
        {
            get { return _rollOff; }
            set
            {
                if (value != null && value.Length == 0)
                    value = null;
                if (value == _rollOff) return;
                _rollOff = value;
                OnPropertyChanged("RollOff");
                OnPropertyChanged("RollOffType");
            }
        }

        /// <summary>
        ///     Only used in DVB-S2. 0=Auto, 2=Off, 1=On
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Pilot
        {
            get { return _pilot; }
            set
            {
                if (value != null && value.Length == 0)
                    value = null;
                if (value == _pilot) return;
                _pilot = value;
                OnPropertyChanged("Pilot");
                OnPropertyChanged("PilotType");
            }
        }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Polarization values</see>
        public Enums.DVBSPolarizationType PolarizationType
        {
            get
            {
                switch (Polarization)
                {
                    case "0":
                        return Enums.DVBSPolarizationType.Horizontal;
                    case "1":
                        return Enums.DVBSPolarizationType.Vertical;
                    case "2":
                        return Enums.DVBSPolarizationType.CircularLeft;
                    case "3":
                        return Enums.DVBSPolarizationType.CircularRight;
                    default:
                        return Enums.DVBSPolarizationType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=None , 1=Auto, 2=1/2, 3=2/3, 4=3/4 5=5/6, 6=7/8, 7=3/5, 8=4/5, 9=8/9, 10=9/10
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        public Enums.DVBSFECType FECType
        {
            get
            {
                switch (FEC)
                {
                    case "0":
                        return Enums.DVBSFECType.None;
                    case "1":
                        return Enums.DVBSFECType.Auto;
                    case "2":
                        return Enums.DVBSFECType.F12;
                    case "3":
                        return Enums.DVBSFECType.F23;
                    case "4":
                        return Enums.DVBSFECType.F34;
                    case "5":
                        return Enums.DVBSFECType.F56;
                    case "6":
                        return Enums.DVBSFECType.F78;
                    case "7":
                        return Enums.DVBSFECType.F35;
                    case "8":
                        return Enums.DVBSFECType.F45;
                    case "9":
                        return Enums.DVBSFECType.F89;
                    case "10":
                        return Enums.DVBSFECType.F910;
                    default:
                        return Enums.DVBSFECType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        public Enums.DVBSInversionType InversionType
        {
            get
            {
                switch (Inversion)
                {
                    case "0":
                        return Enums.DVBSInversionType.Auto;
                    case "1":
                        return Enums.DVBSInversionType.On;
                    case "2":
                        return Enums.DVBSInversionType.Off;
                    default:
                        return Enums.DVBSInversionType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">System types</see>
        public Enums.DVBSSystemType SystemType
        {
            get
            {
                switch (System)
                {
                    case null:
                        return Enums.DVBSSystemType.DVBS;
                    case "0":
                        return Enums.DVBSSystemType.DVBS;
                    case "1":
                        return Enums.DVBSSystemType.DVBS2;
                    default:
                        return Enums.DVBSSystemType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=8PSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        public Enums.DVBSModulationType ModulationType
        {
            get
            {
                switch (Modulation)
                {
                    case null:
                        return Enums.DVBSModulationType.Auto;
                    case "0":
                        return Enums.DVBSModulationType.Auto;
                    case "1":
                        return Enums.DVBSModulationType.QPSK;
                    case "2":
                        return Enums.DVBSModulationType.Qam16;
                    case "3":
                        return Enums.DVBSModulationType.PSK8;
                    default:
                        return Enums.DVBSModulationType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=0.35, 1=0.25, 3=0.20
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">RollOff types</see>
        public Enums.DVBSRollOffType RollOffType
        {
            get
            {
                switch (RollOff)
                {
                    case null:
                        return Enums.DVBSRollOffType.X35;
                    case "0":
                        return Enums.DVBSRollOffType.X35;
                    case "1":
                        return Enums.DVBSRollOffType.X25;
                    case "3":
                        return Enums.DVBSRollOffType.X20;
                    default:
                        return Enums.DVBSRollOffType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 2=Off, 1=On
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Pilot types</see>
        public Enums.DVBSPilotType PilotType
        {
            get
            {
                switch (Pilot)
                {
                    case null:
                        return Enums.DVBSPilotType.Auto;
                    case "0":
                        return Enums.DVBSPilotType.Auto;
                    case "1":
                        return Enums.DVBSPilotType.On;
                    case "2":
                        return Enums.DVBSPilotType.Off;
                    default:
                        return Enums.DVBSPilotType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Calculated NameSpace value which can differ from NameSpc value
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Calculates namespace based on Enigma algorithm</remarks>
        public string CalculatedNameSpace
        {
            get
            {
                int orbPosition = (OrbitalPositionInt > 0 ? OrbitalPositionInt : 3600 + OrbitalPositionInt);
                int dvbNamespace = orbPosition << 16;
                if (IsValidOnidTsid()) return dvbNamespace.ToString("X").PadLeft(8, '0').ToLower();
                if (Frequency != null)
                    dvbNamespace = dvbNamespace | ((Convert.ToInt32(Frequency)/1000) & 0xffff) | ((Convert.ToInt32(Polarization) & 1) << 15);
                return dvbNamespace.ToString("X").PadLeft(8, '0').ToLower();
            }
        }

        /// <summary>
        ///     Checks if transponder has valid TSID and NID
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected bool IsValidOnidTsid()
        {
            int mOnid = Int32.Parse(NID, NumberStyles.HexNumber);
            int mTSID = Int32.Parse(TSID, NumberStyles.HexNumber);
            int orbPosition = (OrbitalPositionInt > 0 ? OrbitalPositionInt : 3600 + OrbitalPositionInt);
            switch (mOnid)
            {
                case 0:
                    return false;
                case 0x1111:
                    return false;
                case 0x13e:
                    // workaround for 11258H and 11470V on hotbird with same ONID/TSID (&H13E/&H578)
                    return orbPosition != 130 | mTSID != 0x578;
                case 1:
                    return orbPosition == 192;
                case 0xb1:
                    return mTSID != 0xb0;
                case 0xeb:
                    return mTSID != 0x4321;
                case 0x2:
                    return Math.Abs(orbPosition - 282) < 6 & mTSID != 2019;
                    // 12070H and 10936V have same tsid/onid.. but even the same services are provided
                case 0x2000:
                    return (mTSID != 0x1000);
                case 0x5e:
                    // Sirius 4.8E 12322V and 12226H
                    return Math.Abs(orbPosition - 48) < 3 & mTSID != 1;
                case 10100:
                    // Eutelsat W7 36.0E 11644V and 11652V
                    return orbPosition != 360 | mTSID != 10187;
                case 42:
                    // Tuerksat 42.0E
                    return orbPosition != 420 | (mTSID != 8 & mTSID != 5 & mTSID != 2 & mTSID != 55);
                case 100:
                    // Intelsat 10 68.5E 3808V 3796V 4012V, Amos 4.0W 10723V 11571H
                    return (orbPosition != 685 & orbPosition != 3560) | mTSID != 1;
                case 70:
                    // Thor 0.8W 11862H 12341V
                    return Math.Abs(orbPosition - 3592) < 3 & mTSID != 46;
                case 32:
                    // NSS 806 (40.5W) 4059R, 3774L
                    return orbPosition != 3195 | mTSID != 21;
                default:
                    return mOnid < 0xff00;
            }
        }

        public override string ToString()
        {
            string tType = string.Join("", new[]
            {
                "\t",
                "s",
                " ",
                Frequency
            });
            if (SystemType == Enums.DVBSSystemType.DVBS & ModulationType == Enums.DVBSModulationType.Auto)
            {
                return string.Join("\t", new[]
                {
                    string.Join(":", new[]
                    {
                        NameSpc.PadRight(8, '0'),
                        TSID.PadLeft(4, '0'),
                        NID.PadLeft(4, '0')
                    }),
                    string.Join(":", new[]
                    {
                        tType,
                        SymbolRate.PadRight(8, '0'),
                        Polarization,
                        FEC,
                        OrbitalPositionInt.ToString(CultureInfo.InvariantCulture),
                        Inversion,
                        Flags
                    }),
                    "/"
                }).TrimEnd(':');
            }
            if (SystemType == Enums.DVBSSystemType.DVBS)
            {
                return string.Join("\t", new[]
                {
                    string.Join(":", new[]
                    {
                        NameSpc.PadRight(8, '0'),
                        TSID.PadLeft(4, '0'),
                        NID.PadLeft(4, '0')
                    }),
                    string.Join(":", new[]
                    {
                        tType,
                        SymbolRate.PadRight(8, '0'),
                        Polarization,
                        FEC,
                        OrbitalPositionInt.ToString(CultureInfo.InvariantCulture),
                        Inversion,
                        Flags,
                        System,
                        Modulation
                    }),
                    "/"
                }).TrimEnd(':');
            }
            return string.Join("\t", new[]
            {
                string.Join(":", new[]
                {
                    NameSpc.PadRight(8, '0'),
                    TSID.PadLeft(4, '0'),
                    NID.PadLeft(4, '0')
                }),
                string.Join(":", new[]
                {
                    tType,
                    SymbolRate.PadRight(8, '0'),
                    Polarization,
                    FEC,
                    OrbitalPositionInt.ToString(CultureInfo.InvariantCulture),
                    Inversion,
                    Flags,
                    System,
                    Modulation,
                    RollOff,
                    Pilot
                }),
                "/"
            }).TrimEnd(':');
        }
    }
}