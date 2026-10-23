// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class XmlCable : IXmlCable
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (XmlCable)MemberwiseClone();
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

        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var cab = (IXmlCable)CloneWithoutSubscribers();
            cab.Transponders = new List<IXmlTransponder>();

            foreach (var xmlTransponder in Transponders)
            {
                if (xmlTransponder != null)
                    cab.Transponders.Add((IXmlTransponder)xmlTransponder.Clone());
            }

            return cab;
        }

        /// <summary>
        ///     Cable flags from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Flags
        {
            get;
            set
            {
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Flags));
            }
        }

        /// <summary>
        ///     Cable name from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Name
        {
            get;
            set
            {
                value ??= string.Empty;

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Name));
            }
        } = string.Empty;

        /// <summary>
        ///     Cable satfeed from cables.xml file
        /// </summary>
        /// <value>true/false</value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string SatFeed
        {
            get;
            set
            {
                value ??= string.Empty;

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(SatFeed));
            }
        } = string.Empty;

        /// <summary>
        ///     Cable countrycode from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string CountryCode
        {
            get;
            set
            {
                value ??= string.Empty;

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(CountryCode));
            }
        } = string.Empty;

        /// <summary>
        ///     List of transponders from xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IList<IXmlTransponder> Transponders
        {
            get;
            set
            {
                value ??= new List<IXmlTransponder>();
                field = value;
                OnPropertyChanged(nameof(Transponders));
            }
        } = new List<IXmlTransponder>();

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