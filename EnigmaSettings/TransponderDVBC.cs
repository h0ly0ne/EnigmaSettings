// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Runtime.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class TransponderDVBC : Transponder, ITransponderDVBC
    {

        #region "IEditable"

        private bool _isEditing;
        private string _mFecInner;
        private string _mFlags;
        private string _mInversion;
        private string _mModulation;
        private string _mSymbolRate;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) return;
            _mFecInner = _fecInner;
            _mFlags = _flags;
            _mInversion = _inversion;
            _mModulation = _modulation;
            _mSymbolRate = _symbolRate;
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
            FECInner = _mFecInner;
            Flags = _mFlags;
            Inversion = _mInversion;
            Modulation = _mModulation;
            SymbolRate = _mSymbolRate;
            _isEditing = false;
        }

        #endregion

        private string _fecInner = "0";
        private string _flags = "0";
        private string _inversion = "0";
        private string _modulation = "0";
        private string _symbolRate = "0";

        /// <summary>
        ///     Initializes new cable transponder from data found in services file
        /// </summary>
        /// <param name="transponderData">0xNamespace:0xTSID:0xNID</param>
        /// <param name="transponderFrequency">c freq(khz):symbolrate(hz):inversion:modulation:fecInner:flags</param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">
        ///     Throws argument null exception if transponderData or transponderFrequency are
        ///     null/empty
        /// </exception>
        /// <exception cref="ArgumentException">Throws argument exception if frequency does not start with 'c'</exception>
        public TransponderDVBC(string transponderData, string transponderFrequency)
        {
            if (string.IsNullOrEmpty(transponderData))
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(transponderFrequency))
                throw new ArgumentNullException();
            _TransponderType = Enums.TransponderType.DVBC;
            string[] tData = transponderData.Split(':');
            string[] tFreq = transponderFrequency.Split(':');

            NameSpc = tData[0];
            TSID = tData[1];
            NID = tData[2];
            if (transponderFrequency.Trim().ToLower().StartsWith("s"))
            {
                Frequency = tFreq[0].Split(' ')[1];
                SymbolRate = tFreq[1];
                if (tFreq.Length > 3)
                    FECInner = tFreq[3];
                if (tFreq.Length > 5)
                    Inversion = tFreq[5];
            }
            else
            {
                Frequency = tFreq[0].Split(' ')[1].Trim();
                SymbolRate = tFreq[1];
                if (tFreq.Length > 2)
                    Inversion = tFreq[2];
                if (tFreq.Length > 3)
                    Modulation = tFreq[3];
                if (tFreq.Length > 4)
                    FECInner = tFreq[4];
                if (tFreq.Length > 5)
                    Flags = tFreq[5];
            }
        }

        /// <summary>
        ///     Transponder symbol rate
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string SymbolRate
        {
            get { return _symbolRate; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _symbolRate) return;
                _symbolRate = value;
                OnPropertyChanged("SymbolRate");
            }
        }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        [DataMember]
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
        ///     0=Auto, 1=QAM16, 2=QAM32, 3=QAM64, 4=QAM128, 5=QAM256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        [DataMember]
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
        ///     0=None, 1=Auto, 2=1/2, 3=2/3, 4=3/4, 5=5/6, 6=7/8, 7=8/9
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        [DataMember]
        public string FECInner
        {
            get { return _fecInner; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _fecInner) return;
                _fecInner = value;
                OnPropertyChanged("FECInner");
                OnPropertyChanged("FECInnerType");
            }
        }

        /// <summary>
        ///     Transponder flag value
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Flags
        {
            get { return _flags; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _flags) return;
                _flags = value;
                OnPropertyChanged("Flags");
            }
        }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        [DataMember]
        public Enums.DVBCInversionType InversionType
        {
            get
            {
                switch (Inversion)
                {
                    case "0":
                        return Enums.DVBCInversionType.Auto;
                    case "1":
                        return Enums.DVBCInversionType.On;
                    case "2":
                        return Enums.DVBCInversionType.Off;
                    default:
                        return Enums.DVBCInversionType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=Auto, 1=QAM16, 2=QAM32, 3=QAM64, 4=QAM128, 5=QAM256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        [DataMember]
        public Enums.DVBCModulationType ModulationType
        {
            get
            {
                switch (Modulation)
                {
                    case "0":
                        return Enums.DVBCModulationType.Auto;
                    case "1":
                        return Enums.DVBCModulationType.Qam16;
                    case "2":
                        return Enums.DVBCModulationType.Qam32;
                    case "3":
                        return Enums.DVBCModulationType.Qam64;
                    case "4":
                        return Enums.DVBCModulationType.Qam128;
                    case "5":
                        return Enums.DVBCModulationType.Qam256;
                    default:
                        return Enums.DVBCModulationType.Unknown;
                }
            }
        }

        /// <summary>
        ///     0=None, 1=Auto, 2=1/2, 3=2/3, 4=3/4, 5=5/6, 6=7/8, 7=8/9
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        [DataMember]
        public Enums.DVBCFECInnerType FECInnerType
        {
            get
            {
                switch (FECInner)
                {
                    case "0":
                        return Enums.DVBCFECInnerType.None;
                    case "1":
                        return Enums.DVBCFECInnerType.Auto;
                    case "2":
                        return Enums.DVBCFECInnerType.F12;
                    case "3":
                        return Enums.DVBCFECInnerType.F23;
                    case "4":
                        return Enums.DVBCFECInnerType.F34;
                    case "5":
                        return Enums.DVBCFECInnerType.F56;
                    case "6":
                        return Enums.DVBCFECInnerType.F78;
                    case "7":
                        return Enums.DVBCFECInnerType.F89;
                    default:
                        return Enums.DVBCFECInnerType.Unknown;
                }
            }
        }

        public override string ToString()
        {
            string tType = string.Join("", new[]
            {
                "\t",
                "c",
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
                    SymbolRate,
                    Inversion,
                    Modulation,
                    FECInner,
                    Flags
                }),
                "/"
            });
        }
    }
}