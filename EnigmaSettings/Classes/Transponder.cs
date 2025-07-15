// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System.ComponentModel;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public abstract class Transponder : ITransponder
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        protected Enums.TransponderType _TransponderType = Enums.TransponderType.Unknown;
        private string _frequency = "0";
        private string _nameSpc = "0";
        private string _nid = "0";
        private string _tsid = "0";

        /// <summary>
        ///     Namespace of the transponder
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match transponder</remarks>
        [DataMember]
        public string NameSpc
        {
            get => _nameSpc;
            set
            {
                value ??= "0";
                if (value == _nameSpc) 
                    return;

                _nameSpc = value;
                OnPropertyChanged(nameof(NameSpc));
                OnPropertyChanged(nameof(TransponderId));
            }
        }

        /// <summary>
        ///     Transponder ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Should be unique, but often not</remarks>
        [DataMember]
        public string TSID
        {
            get => _tsid;
            set
            {
                value ??= "0";
                if (value == _tsid) 
                    return;

                _tsid = value;
                OnPropertyChanged(nameof(TSID));
                OnPropertyChanged(nameof(TransponderId));
            }
        }

        /// <summary>
        ///     Original network ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string NID
        {
            get => _nid;
            set
            {
                value ??= "0";
                if (value == _nid) 
                    return;

                _nid = value;
                OnPropertyChanged(nameof(NID));
                OnPropertyChanged(nameof(TransponderId));
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
                value ??= "0";
                if (value == _frequency)
                    return;

                _frequency = value;
                OnPropertyChanged(nameof(Frequency));
            }
        }

        /// <summary>
        ///     Transponder type (Satellite, Cable, Terrestrial)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public Enums.TransponderType TransponderType => _TransponderType;

        /// <summary>
        ///     0xNamespace:0xTSID:0xNID
        /// </summary>
        /// <value></value>
        /// <returns>Returns 1st line of transponder info for services file</returns>
        /// <remarks>Used to match transponder to service</remarks>
        [DataMember]
        public string TransponderId => string.Join(":", NameSpc, TSID, NID).ToLower();

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