// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public class TransponderDVBT : Transponder, ITransponderDVBT
    {

        #region "IEditable"

        private bool _isEditing;
        private string _mBandwidth;
        private string _mFecHigh;
        private string _mFecLow;
        private string _mFlags;
        private string _mGuardInterval;
        private string _mHierarchy;
        private string _mInversion;
        private string _mModulation;
        private string _mTransmission;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) return;
            _mBandwidth = _bandwidth;
            _mFecHigh = _fecHigh;
            _mFecLow = _fecLow;
            _mFlags = _flags;
            _mGuardInterval = _guardInterval;
            _mHierarchy = _hierarchy;
            _mInversion = _inversion;
            _mModulation = _modulation;
            _mTransmission = _transmission;
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
            Bandwidth = _mBandwidth;
            FECHigh = _mFecHigh;
            FECLow = _mFecLow;
            Flags = _mFlags;
            GuardInterval = _mGuardInterval;
            Hierarchy = _mHierarchy;
            Inversion = _mInversion;
            Modulation = _mModulation;
            Transmission = _mTransmission;
            _isEditing = false;
        }

        #endregion
       
        private string _bandwidth = "0";
        private string _fecHigh = "0";
        private string _fecLow = "0";
        private string _flags = "0";
        private string _guardInterval = "0";
        private string _hierarchy = "0";
        private string _inversion = "0";
        private string _modulation = "0";
        private string _transmission = "0";

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
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(transponderFrequency))
                throw new ArgumentNullException();
            _TransponderType = Enums.TransponderType.DVBT;
            string[] tData = transponderData.Split(':');
            string[] tFreq = transponderFrequency.Split(':');

            NameSpc = tData[0];
            TSID = tData[1];
            NID = tData[2];

            if (transponderFrequency.Trim().ToLower().StartsWith("s"))
            {
                Frequency = tFreq[0].Split(' ')[1];
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
            }
        }

        /// <summary>
        ///     0=Auto, 1=8Mhz, 2=7Mhz, 3=6Mhz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Bandwidth values</see>
        public string Bandwidth
        {
            get { return _bandwidth; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _bandwidth) return;
                _bandwidth = value;
                OnPropertyChanged("Bandwidth");
                OnPropertyChanged("BandwidthType");
            }
        }

        /// <summary>
        ///     Code rate High Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        public string FECHigh
        {
            get { return _fecHigh; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _fecHigh) return;
                _fecHigh = value;
                OnPropertyChanged("FECHigh");
                OnPropertyChanged("FECHighType");
            }
        }

        /// <summary>
        ///     Code rate Low Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        public string FECLow
        {
            get { return _fecLow; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _fecLow) return;
                _fecLow = value;
                OnPropertyChanged("FECLow");
                OnPropertyChanged("FECLowType");
            }
        }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=QAM64
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
                if (value == null)
                    value = "0";
                if (value == _modulation) return;
                _modulation = value;
                OnPropertyChanged("Modulation");
                OnPropertyChanged("ModulationType");
            }
        }

        /// <summary>
        ///     0=Auto, 1=2k, 3=8k
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Transmission types</see>
        public string Transmission
        {
            get { return _transmission; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _transmission) return;
                _transmission = value;
                OnPropertyChanged("Transmission");
                OnPropertyChanged("TransmissionType");
            }
        }

        /// <summary>
        ///     0=Auto, 1=1/32, 2=1/16, 3=1/8, 4=1/4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Guard interval types</see>
        public string GuardInterval
        {
            get { return _guardInterval; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _guardInterval) return;
                _guardInterval = value;
                OnPropertyChanged("GuardInterval");
                OnPropertyChanged("GuardIntervalType");
            }
        }

        /// <summary>
        ///     0=Auto, 1=None, 2=1, 3=2, 4=4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Hierarchy types</see>
        public string Hierarchy
        {
            get { return _hierarchy; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _hierarchy) return;
                _hierarchy = value;
                OnPropertyChanged("Hierarchy");
                OnPropertyChanged("HierarchyType");
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
        ///     Transponder flags
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Flags
        {
            get { return _flags; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == Flags) return;
                _flags = value;
                OnPropertyChanged("Flags");
            }
        }

        /// <summary>
        ///     0=Auto, 1=8Mhz, 2=7Mhz, 3=6Mhz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Bandwidth types</see>
        public Enums.DVBTBandwidthType BandwidthType
        {
            get
            {
                switch (Bandwidth)
                {
                    case "0":
                        return Enums.DVBTBandwidthType.Auto;
                    case "1":
                        return Enums.DVBTBandwidthType.B8Mhz;
                    case "2":
                        return Enums.DVBTBandwidthType.B7Mhz;
                    case "3":
                        return Enums.DVBTBandwidthType.B6Mhz;
                    default:
                        return Enums.DVBTBandwidthType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Code rate High Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        public Enums.DVBTFECHighType FECHighType
        {
            get
            {
                switch (FECHigh)
                {
                    case "0":
                        return Enums.DVBTFECHighType.Auto;
                    case "1":
                        return Enums.DVBTFECHighType.F12;
                    case "2":
                        return Enums.DVBTFECHighType.F23;
                    case "3":
                        return Enums.DVBTFECHighType.F34;
                    case "4":
                        return Enums.DVBTFECHighType.F56;
                    case "5":
                        return Enums.DVBTFECHighType.F78;
                    default:
                        return Enums.DVBTFECHighType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Code rate Low Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        public Enums.DVBTFECLowType FECLowType
        {
            get
            {
                switch (FECLow)
                {
                    case "0":
                        return Enums.DVBTFECLowType.Auto;
                    case "1":
                        return Enums.DVBTFECLowType.F12;
                    case "2":
                        return Enums.DVBTFECLowType.F23;
                    case "3":
                        return Enums.DVBTFECLowType.F34;
                    case "4":
                        return Enums.DVBTFECLowType.F56;
                    case "5":
                        return Enums.DVBTFECLowType.F78;
                    default:
                        return Enums.DVBTFECLowType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=QAM64
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        public Enums.DVBTModulationType ModulationType
        {
            get
            {
                switch (Modulation)
                {
                    case "0":
                        return Enums.DVBTModulationType.Auto;
                    case "1":
                        return Enums.DVBTModulationType.QPSK;
                    case "2":
                        return Enums.DVBTModulationType.Qam16;
                    case "3":
                        return Enums.DVBTModulationType.Qam64;
                    default:
                        return Enums.DVBTModulationType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=2k, 3=8k
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Transmission types</see>
        public Enums.DVBTTransmissionType TransmissionType
        {
            get
            {
                switch (Transmission)
                {
                    case "0":
                        return Enums.DVBTTransmissionType.Auto;
                    case "1":
                        return Enums.DVBTTransmissionType.T2K;
                    case "3":
                        return Enums.DVBTTransmissionType.T8K;
                    default:
                        return Enums.DVBTTransmissionType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=1/32, 2=1/16, 3=1/8, 4=1/4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Guard interval types</see>
        public Enums.DVBTGuardIntervalType GuardIntervalType
        {
            get
            {
                switch (GuardInterval)
                {
                    case "0":
                        return Enums.DVBTGuardIntervalType.Auto;
                    case "1":
                        return Enums.DVBTGuardIntervalType.G132;
                    case "2":
                        return Enums.DVBTGuardIntervalType.G116;
                    case "3":
                        return Enums.DVBTGuardIntervalType.G18;
                    case "4":
                        return Enums.DVBTGuardIntervalType.G14;
                    default:
                        return Enums.DVBTGuardIntervalType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=None, 2=1, 3=2, 4=4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Hierarchy types</see>
        public Enums.DVBTHierarchyType HierarchyType
        {
            get
            {
                switch (Hierarchy)
                {
                    case "0":
                        return Enums.DVBTHierarchyType.Auto;
                    case "1":
                        return Enums.DVBTHierarchyType.None;
                    case "2":
                        return Enums.DVBTHierarchyType.H1;
                    case "3":
                        return Enums.DVBTHierarchyType.H2;
                    case "4":
                        return Enums.DVBTHierarchyType.H4;
                    default:
                        return Enums.DVBTHierarchyType.Unknown;
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
        public Enums.DVBTInversionType InversionType
        {
            get
            {
                switch (Inversion)
                {
                    case "0":
                        return Enums.DVBTInversionType.Auto;
                    case "1":
                        return Enums.DVBTInversionType.On;
                    case "2":
                        return Enums.DVBTInversionType.Off;
                    default:
                        return Enums.DVBTInversionType.Unknown;
                }
            }
        }

        public override string ToString()
        {
            string tType = string.Join("", new[]
            {
                "\t",
                "t",
                " ",
                Frequency
            });
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
                    Bandwidth,
                    FECHigh,
                    FECLow,
                    Modulation,
                    Transmission,
                    GuardInterval,
                    Hierarchy,
                    Inversion,
                    Flags
                }),
                "/"
            });
        }

    }
}