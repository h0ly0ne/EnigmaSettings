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
        private string _mFec;
        private string _mFlags;
        private string _mInversion;
        private string _mModulation;
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
            SymbolRate = _mSymbolRate;
            System = _mSystem;
            _isEditing = false;
        }
        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            var transponder = (ITransponderDVBC)MemberwiseClone();
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
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(transponderFrequency))
                throw new ArgumentNullException();

            _TransponderType = Enums.TransponderType.DVBC;
            string[] tData = transponderData.Split(':');
            string[] tFreq = transponderFrequency.Split(':');

            NameSpc = tData[0];
            TSID = tData[1];
            NID = tData[2];

            if (transponderFrequency.Trim().ToLower().StartsWith('c'))
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
        }

        /// <summary>
        ///     Symbol rate
        /// </summary>
        [DataMember]
        public string SymbolRate
        {
            get => _symbolRate;
            set
            {
                value ??= "0";
                if (value == _symbolRate)
                    return;

                _symbolRate = value;
                OnPropertyChanged(nameof(SymbolRate));
            }
        }
        private string _symbolRate;

        /// <summary>
        ///     Spectral inversion
        /// </summary>
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
        ///     Spectral inversion
        /// </summary>
        [DataMember]
        public string Inversion
        {
            get => _inversion;
            set
            {
                value ??= "0";
                if (value == _inversion)
                    return;

                _inversion = value;
                OnPropertyChanged(nameof(Inversion));
                OnPropertyChanged(nameof(InversionType));
            }
        }
        private string _inversion;

        /// <summary>
        ///     Modulation
        /// </summary>
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
        ///     Modulation
        /// </summary>
        [DataMember]
        public string Modulation
        {
            get => _modulation;
            set
            {
                value ??= "0";
                if (value == _modulation) 
                    return;

                _modulation = value;
                OnPropertyChanged(nameof(Modulation));
                OnPropertyChanged(nameof(ModulationType));
            }
        }
        private string _modulation;

        /// <summary>
        ///     FEC
        /// </summary>
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
        ///     FEC
        /// </summary>
        [DataMember]
        public string FEC
        {
            get => _fec;
            set
            {
                value ??= "0";
                if (value == _fec)
                    return;

                _fec = value;
                OnPropertyChanged(nameof(FEC));
                OnPropertyChanged(nameof(FECType));
            }
        }
        private string _fec;

        /// <summary>
        ///     Flag value
        /// </summary>
        [DataMember]
        public string Flags
        {
            get => _flags;
            set
            {
                value ??= "0";
                if (value == _flags)
                    return;

                _flags = value;
                OnPropertyChanged(nameof(Flags));
            }
        }
        private string _flags;

        /// <summary>
        ///     DVB system
        /// </summary>
        [DataMember]
        public string System
        {
            get => _system;
            set
            {
                if (value == _system) 
                    return;

                _system = value;
                OnPropertyChanged(nameof(System));
            }
        }
        private string _system;

        public override string ToString()
        {
            var tType = string.Join("", "\t", "c", " ", Frequency);
            return string.Join("\t", string.Join(":", NameSpc.PadRight(8, '0'), TSID.PadLeft(4, '0'), NID.PadLeft(4, '0')), string.Join(":", tType, SymbolRate, Inversion, Modulation, FEC, Flags, System), "/");
        }
    }
}