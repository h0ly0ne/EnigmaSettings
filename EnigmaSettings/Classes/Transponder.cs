// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
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

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (Transponder)MemberwiseClone();
            clone.PropertyChanged = null;
            clone._editSnapshot = null;
            return clone;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region "IEditable"
        // field values captured by BeginEdit (see EditSnapshot), null while not editing
        private object[] _editSnapshot;

        public virtual void BeginEdit()
        {
            _editSnapshot ??= EditSnapshot.Capture(this);
        }

        public virtual void EndEdit()
        {
            _editSnapshot = null;
        }

        public virtual void CancelEdit()
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
        public virtual object Clone()
        {
            return CloneWithoutSubscribers();
        }
        #endregion

        protected Enums.TransponderType _TransponderType = Enums.TransponderType.Unknown;

        /// <summary>
        ///     Namespace of the transponder
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match transponder</remarks>
        [DataMember]
        public string NameSpc
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                _transponderId = null;
                OnPropertyChanged(nameof(NameSpc));
                OnPropertyChanged(nameof(TransponderId));
            }
        } = "0";

        /// <summary>
        ///     Transponder ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Should be unique, but often not</remarks>
        [DataMember]
        public string TSID
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                _transponderId = null;
                OnPropertyChanged(nameof(TSID));
                OnPropertyChanged(nameof(TransponderId));
            }
        } = "0";

        /// <summary>
        ///     Original network ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string NID
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                _transponderId = null;
                OnPropertyChanged(nameof(NID));
                OnPropertyChanged(nameof(TransponderId));
            }
        } = "0";

        /// <summary>
        ///     Frequency in Hertz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Frequency
        {
            get;
            set
            {
                value ??= "0";

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Frequency));
            }
        } = "0";

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
        public string TransponderId => _transponderId ??= string.Join(":", NameSpc, TSID, NID).ToLowerInvariant();

        // cached, used as join key for every service while loading and matching
        private string _transponderId;

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