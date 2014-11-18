// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.ComponentModel;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public abstract class Transponder : ITransponder
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
        private string _mFrequency;
        private string _mNameSpc;
        private string _mNid;
        private string _mTsid;

        public virtual void BeginEdit()
        {
            if (_isEditing) return;
            _mFrequency = _frequency;
            _mNameSpc = _nameSpc;
            _mNid = _nid;
            _mTsid = _tsid;
            _isEditing = true;
        }

        public virtual void EndEdit()
        {
            _isEditing = false;
        }

        public virtual void CancelEdit()
        {
            if (!_isEditing) return;
            Frequency = _mFrequency;
            NameSpc = _mNameSpc;
            NID = _mNid;
            TSID = _mTsid;
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


        // ReSharper disable once InconsistentNaming
        protected Enums.TransponderType _TransponderType = Enums.TransponderType.Unknown;
        private string _frequency = "0";
        private string _nameSpc = "0";
        private string _nid = "0";
        private string _tsid = "0";

        /// <summary>
        ///     Namespace of the satellite
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match transponder to satellite</remarks>
        public string NameSpc
        {
            get { return _nameSpc; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _nameSpc) return;
                _nameSpc = value;
                OnPropertyChanged("NameSpc");
                OnPropertyChanged("TransponderId");
            }
        }

        /// <summary>
        ///     Transponder ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Should be unique for satellite, but often not</remarks>
        public string TSID
        {
            get { return _tsid; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _tsid) return;
                _tsid = value;
                OnPropertyChanged("TSID");
                OnPropertyChanged("TransponderId");
            }
        }

        /// <summary>
        ///     Original network ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string NID
        {
            get { return _nid; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _nid) return;
                _nid = value;
                OnPropertyChanged("NID");
                OnPropertyChanged("TransponderId");
            }
        }

        /// <summary>
        ///     Frequency in Hertz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Frequency
        {
            get { return _frequency; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _frequency) return;
                _frequency = value;
                OnPropertyChanged("Frequency");
            }
        }

        // ReSharper disable once InconsistentNaming

        /// <summary>
        ///     Transponder type (Satellite, Cable, Terrestrial)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Enums.TransponderType TransponderType
        {
            get { return _TransponderType; }
        }

        /// <summary>
        ///     0xNamespace:0xTSID:0xNID
        /// </summary>
        /// <value></value>
        /// <returns>Returns 1st line of transponder info for services file</returns>
        /// <remarks>Used to match transponder to service</remarks>
        public string TransponderId
        {
            get { return string.Join(":", new[] { NameSpc, TSID, NID }).ToLower(); }
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