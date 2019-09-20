// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class XmlTransponder : IXmlTransponder
    {
        #region "INotifyPropertyChanged"

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region "IEditable"

        private bool _isEditing;
        private string _mFecInner;
        private string _mFrequency;
        private string _mInversion;
        private string _mModulation;
        private string _mPilot;
        private string _mPolarization;
        private string _mRollOff;
        private string _mSymbolRate;
        private string _mSystem;
        private string _mIs_id;
        private string _mPls_code;
        private string _mPls_mode;
        private string _mT2mi_plp_id;
        private string _mT2mi_pid;


        public void BeginEdit()
        {
            if (_isEditing) return;
            _mFecInner = _fecInner;
            _mFrequency = _frequency;
            _mInversion = _inversion;
            _mModulation = _modulation;
            _mPilot = _pilot;
            _mPolarization = _polarization;
            _mRollOff = _rollOff;
            _mSymbolRate = _symbolRate;
            _mSystem = _system;
            _mIs_id = _is_id;
            _mPls_code = _pls_code;
            _mPls_mode = _pls_mode;
            _mT2mi_plp_id = _t2mi_plp_id;
            _mT2mi_pid = _t2mi_pid;
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing) return;
            FECInner = _mFecInner;
            Frequency = _mFrequency;
            Inversion = _mInversion;
            Modulation = _mModulation;
            Pilot = _mPilot;
            Polarization = _mPolarization;
            RollOff = _mRollOff;
            SymbolRate = _mSymbolRate;
            System = _mSystem;
            IsId = _mIs_id;
            PlsCode = _mPls_code;
            PlsMode = _mPls_mode;
            T2miPlpId = _mT2mi_plp_id;
            T2miPid = _mT2mi_pid;
            _isEditing = false;
        }

        #endregion

        #region "ICloneable"

        /// <summary>
        /// Performs Memberwise Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        private string _fecInner;
        private string _frequency;
        private string _inversion;
        private string _modulation;
        private string _pilot;
        private string _polarization;
        private string _rollOff;
        private string _symbolRate;
        private string _system;
        private string _is_id;
        private string _pls_code;
        private string _pls_mode;
        private string _t2mi_plp_id;
        private string _t2mi_pid;

        /// <summary>
        ///     0=None , 1=Auto, 2=1/2, 3=2/3, 4=3/4 5=5/6, 6=7/8, 7=3/5, 8=4/5, 9=8/9, 10=9/10
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
                if (value == _fecInner) return;
                _fecInner = value;
                OnPropertyChanged("FECInner");
            }
        }

        /// <summary>
        ///     Frequency in Hertz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Frequency
        {
            get { return _frequency; }
            set
            {
                if (value == _frequency) return;
                _frequency = value;
                OnPropertyChanged("Frequency");
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
                if (value == _inversion) return;
                _inversion = value;
                OnPropertyChanged("Inversion");
            }
        }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=8PSK
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
                if (value == _modulation) return;
                _modulation = value;
                OnPropertyChanged("Modulation");
            }
        }

        /// <summary>
        ///     0=Auto, 1=Off, 1=On
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Pilot types</see>
        [DataMember]
        public string Pilot
        {
            get { return _pilot; }
            set
            {
                if (value == _pilot) return;
                _pilot = value;
                OnPropertyChanged("Pilot");
            }
        }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Polarization values</see>
        [DataMember]
        public string Polarization
        {
            get { return _polarization; }
            set
            {
                if (value == _polarization) return;
                _polarization = value;
                OnPropertyChanged("Polarization");
            }
        }

        /// <summary>
        ///     0=0.35, 1=0.25, 3=0.20
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">RollOff types</see>
        [DataMember]
        public string RollOff
        {
            get { return _rollOff; }
            set
            {
                if (value == _rollOff) return;
                _rollOff = value;
                OnPropertyChanged("RollOff");
            }
        }

        /// <summary>
        ///     Symbol rate
        /// </summary>
        /// <value>Usually 8 digit integer</value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string SymbolRate
        {
            get { return _symbolRate; }
            set
            {
                if (value == _symbolRate) return;
                _symbolRate = value;
                OnPropertyChanged("SymbolRate");
            }
        }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">System types</see>
        [DataMember]
        public string System
        {
            get { return _system; }
            set
            {
                if (value == _system) return;
                _system = value;
                OnPropertyChanged("System");
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string IsId
        {
            get { return _is_id; }
            set
            {
                if (value == _is_id) return;
                _is_id = value;
                OnPropertyChanged("IsId");
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string PlsCode
        {
            get { return _pls_code; }
            set
            {
                if (value == _pls_code) return;
                _pls_code = value;
                OnPropertyChanged("PlsCode");
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string PlsMode
        {
            get { return _pls_mode; }
            set
            {
                if (value == _pls_mode) return;
                _pls_mode = value;
                OnPropertyChanged("PlsMode");
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string T2miPlpId
        {
            get { return _t2mi_plp_id; }
            set
            {
                if (value == _t2mi_plp_id) return;
                _t2mi_plp_id = value;
                OnPropertyChanged("T2miPlpId");
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string T2miPid
        {
            get { return _t2mi_pid; }
            set
            {
                if (value == _t2mi_pid) return;
                _t2mi_pid = value;
                OnPropertyChanged("T2miPid");
            }
        }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        public object ShallowCopy()
        {
            return MemberwiseClone();
        }

    }
}