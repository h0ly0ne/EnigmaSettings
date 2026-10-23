// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

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

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (XmlTransponder)MemberwiseClone();
            clone.PropertyChanged = null;
            clone._editSnapshot = null;
            return clone;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region "IEditable"
        // field values captured by BeginEdit (see EditSnapshot), null while not editing
        private object[] _editSnapshot;

        public void BeginEdit()
        {
            _editSnapshot ??= EditSnapshot.Capture(this);
        }

        public void EndEdit()
        {
            _editSnapshot = null;
        }

        public void CancelEdit()
        {
            if (_editSnapshot == null)
                return;

            EditSnapshot.Restore(this, _editSnapshot);
            _editSnapshot = null;
            OnPropertyChanged(string.Empty);
        }
        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs Memberwise Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return CloneWithoutSubscribers();
        }
        #endregion

        private string _fec;
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
        ///     0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8, 6=8/9, 7=3/5, 8=4/5, 9=9/10, 10=6/7, 15=None
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        [DataMember]
        public string FEC
        {
            get => _fec;
            set
            {
                if (value == _fec) 
                    return;

                _fec = value;
                OnPropertyChanged(nameof(FEC));
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
            get => _frequency;
            set
            {
                if (value == _frequency) 
                    return;

                _frequency = value;
                OnPropertyChanged(nameof(Frequency));
            }
        }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        [DataMember]
        public string Inversion
        {
            get => _inversion;
            set
            {
                if (value == _inversion) 
                    return;

                _inversion = value;
                OnPropertyChanged(nameof(Inversion));
            }
        }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=8PSK, 3=QAM16, 4=16APSK, 5=32APSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        [DataMember]
        public string Modulation
        {
            get => _modulation;
            set
            {
                if (value == _modulation)
                    return;

                _modulation = value;
                OnPropertyChanged(nameof(Modulation));
            }
        }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        [DataMember]
        public string Pilot
        {
            get => _pilot;
            set
            {
                if (value == _pilot) 
                    return;

                _pilot = value;
                OnPropertyChanged(nameof(Pilot));
            }
        }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        [DataMember]
        public string Polarization
        {
            get => _polarization;
            set
            {
                if (value == _polarization) 
                    return;

                _polarization = value;
                OnPropertyChanged(nameof(Polarization));
            }
        }

        /// <summary>
        ///     0=0.35, 1=0.25, 2=0.20, 3=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        [DataMember]
        public string RollOff
        {
            get => _rollOff;
            set
            {
                if (value == _rollOff) 
                    return;

                _rollOff = value;
                OnPropertyChanged(nameof(RollOff));
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
            get => _symbolRate;
            set
            {
                if (value == _symbolRate) 
                    return;

                _symbolRate = value;
                OnPropertyChanged(nameof(SymbolRate));
            }
        }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
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

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string IsId
        {
            get => _is_id;
            set
            {
                if (value == _is_id) 
                    return;

                _is_id = value;
                OnPropertyChanged(nameof(IsId));
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string PlsCode
        {
            get => _pls_code;
            set
            {
                if (value == _pls_code)
                    return;

                _pls_code = value;
                OnPropertyChanged(nameof(PlsCode));
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string PlsMode
        {
            get => _pls_mode;
            set
            {
                if (value == _pls_mode) 
                    return;

                _pls_mode = value;
                OnPropertyChanged(nameof(PlsMode));
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string T2miPlpId
        {
            get => _t2mi_plp_id;
            set
            {
                if (value == _t2mi_plp_id)
                    return;

                _t2mi_plp_id = value;
                OnPropertyChanged(nameof(T2miPlpId));
            }
        }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        [DataMember]
        public string T2miPid
        {
            get => _t2mi_pid;
            set
            {
                if (value == _t2mi_pid)
                    return;

                _t2mi_pid = value;
                OnPropertyChanged(nameof(T2miPid));
            }
        }

        /// <summary>
        ///     Transport stream id (satellites.xml attribute "tsid"), optional
        /// </summary>
        [DataMember]
        public string Tsid
        {
            get => _tsid;
            set
            {
                if (value == _tsid)
                    return;

                _tsid = value;
                OnPropertyChanged(nameof(Tsid));
            }
        }

        /// <summary>
        ///     Original network id (satellites.xml attribute "onid"), optional
        /// </summary>
        [DataMember]
        public string Onid
        {
            get => _onid;
            set
            {
                if (value == _onid)
                    return;

                _onid = value;
                OnPropertyChanged(nameof(Onid));
            }
        }

        private string _tsid;
        private string _onid;

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        public object ShallowCopy()
        {
            return CloneWithoutSubscribers();
        }
    }
}